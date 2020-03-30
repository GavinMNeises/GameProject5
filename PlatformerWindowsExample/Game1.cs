using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PlatformLibrary;

namespace PlatformerExample
{
    enum GameState
    {
        Start,
        Playing,
        End
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteSheet sheet;
        Tileset tileset;
        Tilemap tilemap;
        Player player;
        
        List<Platform> platforms;
        AxisList platformAxis;

        List<Token> tokens;
        AxisList tokenAxis;

        ParticleSystem playerDeathParticles;

        GameText gameText;
        GameState state;
        int score;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            platforms = new List<Platform>();
            tokens = new List<Token>();
            gameText = new GameText();

            state = GameState.Start;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
#if VISUAL_DEBUG
            VisualDebugging.LoadContent(Content);
#endif
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var t = Content.Load<Texture2D>("spritesheet");
            sheet = new SpriteSheet(t, 21, 21, 3, 2);

            // Load the tilemap
            tilemap = Content.Load<Tilemap>("level2");

            SoundEffect tokenSound = Content.Load<SoundEffect>("PickupCoin");

            SoundEffect playerJump = Content.Load<SoundEffect>("Jump");
            SoundEffect playerHurt = Content.Load<SoundEffect>("Hurt");

            //Gets the texture for the particle systems and initializes them
            Texture2D texture = Content.Load<Texture2D>("Particle");
            playerDeathParticles = new ParticleSystem(GraphicsDevice, 1000, texture);
            playerDeathParticles.SpawnPerFrame = 20;

            ParticleSystem coinCollectParticles = new ParticleSystem(GraphicsDevice, 1000, texture);
            coinCollectParticles.SpawnPerFrame = 20;

            ParticleSystem playerJumpParticles = new ParticleSystem(GraphicsDevice, 1000, texture);
            playerJumpParticles.SpawnPerFrame = 20;

            //Set the SpawnParticle method for the systems
            playerDeathParticles.SpawnParticle = (ref Particle particle) =>
            {
                Random random = new Random();
                particle.Position = new Vector2(player.Position.X, player.Position.Y - 20);
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-75, 75, (float)random.NextDouble()), //X between -75 and 75
                    MathHelper.Lerp(0, -150, (float)random.NextDouble()) //Y between 0 and -150
                );
                particle.Acceleration = 0.1f * new Vector2(0, (float)random.NextDouble());
                particle.Color = Color.Red;
                particle.Scale = 1f;
                particle.Life = 1.0f;
            };

            coinCollectParticles.SpawnParticle = (ref Particle particle) =>
            {
                Random random = new Random();
                particle.Position = coinCollectParticles.Emitter;
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-180, 180, (float)random.NextDouble()), //X between -180 and 180
                    MathHelper.Lerp(-180, 180, (float)random.NextDouble()) //Y between 180 and 180
                );
                particle.Acceleration = new Vector2(0, 0);
                particle.Color = Color.Gold;
                particle.Scale = 0.5f;
                particle.Life = 0.25f;
            };

            playerJumpParticles.SpawnParticle = (ref Particle particle) =>
            {
                Random random = new Random();
                particle.Position = player.Position;
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-180, 180, (float)random.NextDouble()), //X between -180 and 180
                    MathHelper.Lerp(0, 180, (float)random.NextDouble()) //Y between 0 and 180
                );
                particle.Acceleration = new Vector2(0, 0);
                if (random.NextDouble() > .5)
                {
                    particle.Color = new Color(128, 190, 31);
                }
                else
                {
                    particle.Color = new Color(187, 128, 68);
                }
                particle.Scale = 0.5f;
                particle.Life = 0.25f;
            };

            //Set the UpdateParticle method for the systems
            playerDeathParticles.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= deltaT;
            };

            coinCollectParticles.UpdateParticle = playerDeathParticles.UpdateParticle;
            playerJumpParticles.UpdateParticle = playerDeathParticles.UpdateParticle;

            // Use the object groups to load in the tokens, platforms, and player spawn point
            foreach (ObjectGroup objectGroup in tilemap.ObjectGroups)
            {
                if(objectGroup.Name == "Coin Layer")
                {
                    foreach(GroupObject groupObject in objectGroup.Objects)
                    {
                        BoundingRectangle bounds = new BoundingRectangle(
                            groupObject.X,
                            groupObject.Y,
                            groupObject.Width,
                            groupObject.Height
                        );
                        tokens.Add(new Token(bounds, sheet[groupObject.SheetIndex-1], tokenSound));
                    }
                    tokenAxis = new AxisList();
                    foreach (Token token in tokens)
                    {
                        tokenAxis.AddGameObject(token);
                    }
                }
                else if(objectGroup.Name == "Platform Layer")
                {
                    foreach(GroupObject groupObject in objectGroup.Objects)
                    {
                        BoundingRectangle bounds = new BoundingRectangle(
                            groupObject.X,
                            groupObject.Y,
                            groupObject.Width,
                            groupObject.Height
                        );
                        platforms.Add(new Platform(bounds, sheet[groupObject.SheetIndex - 1]));
                    }

                    platformAxis = new AxisList();
                    foreach (Platform platform in platforms)
                    {
                        platformAxis.AddGameObject(platform);
                    }
                }
                else if(objectGroup.Name == "Spawn Layer")
                {
                    GroupObject groupObject = objectGroup.Objects[0];
                    // Create the player with the corresponding frames from the spritesheet
                    var playerFrames = from index in Enumerable.Range(19, 11) select sheet[index];
                    List<Sprite> playerFramesList = playerFrames.ToList();
                    playerFramesList.Add(sheet[112]);
                    player = new Player(playerFramesList, groupObject.X, groupObject.Y, playerJump, playerHurt, playerJumpParticles, coinCollectParticles);
                }
            }

            score = 0;

            gameText.LoadContent(Content);

            tileset = Content.Load<Tileset>("tiledspritesheetFixed");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (state == GameState.End)
                {
                    player.Reset();
                    tokens.ForEach(token =>
                    {
                        token.Reset();
                    });
                    score = 0;
                }
                state = GameState.Playing;
            }

            if (state == GameState.Playing)
            {
                // TODO: Add your update logic here
                player.Update(gameTime);
                if(player.Dead)
                {
                    state = GameState.End;
                    playerDeathParticles.SystemLife = 0.5f;
                }

                // Check for platform collisions
                var platformQuery = platformAxis.QueryRange(player.Bounds.X, player.Bounds.X + player.Bounds.Width);
                player.CheckForPlatformCollision(platformQuery);

                var tokenQuery = tokenAxis.QueryRange(player.Bounds.X, player.Bounds.X + player.Bounds.Width);
                score += player.CheckForTokenCollision(tokenQuery);
            }

            playerDeathParticles.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Calculate and apply the world/view transform
            var offset = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - player.Position;
            var t = Matrix.CreateTranslation(offset.X, offset.Y, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, t);

            // Draw the tilemap
            tilemap.Draw(spriteBatch);

            // Draw the platforms 
            var platformQuery = platformAxis.QueryRange(player.Position.X - GraphicsDevice.Viewport.Width / 2, player.Position.X + GraphicsDevice.Viewport.Width / 2);
            foreach (Platform platform in platformQuery)
            {
                platform.Draw(spriteBatch);
            }
            Debug.WriteLine($"{platformQuery.Count()} Platforms rendered");

            var tokenQuery = tokenAxis.QueryRange(player.Position.X - GraphicsDevice.Viewport.Width / 2, player.Position.X + GraphicsDevice.Viewport.Width / 2);
            foreach (Token token in tokenQuery)
            {
                token.Draw(spriteBatch);
            }

            // Draw the player
            player.Draw(spriteBatch, t);

            //Draw score
            Vector2 location = new Vector2(GraphicsDevice.Viewport.Width / 2 + player.Position.X, player.Position.Y - GraphicsDevice.Viewport.Height / 2);
            gameText.DrawScore(spriteBatch, "Score: " + score.ToString(), location);

            //Draw start game message if the beginning of the game
            if (state == GameState.Start)
            {
                gameText.Draw(spriteBatch, "Press Enter To Begin", player.Position);
            }

            //If game is over draw the end game and begin game message
            if (state == GameState.End)
            {
                gameText.Draw(spriteBatch, "Press Enter To Begin", player.Position);

                gameText.Draw(spriteBatch, "Game Over!", new Vector2(player.Position.X, player.Position.Y + GraphicsDevice.Viewport.Height / 4));

                playerDeathParticles.Draw(t);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
