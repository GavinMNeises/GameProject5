using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace PlatformerExample
{
    /// <summary>
    /// An enumeration of possible player animation states
    /// </summary>
    enum PlayerAnimState
    {
        Idle,
        JumpingLeft,
        JumpingRight,
        WalkingLeft,
        WalkingRight,
        FallingLeft,
        FallingRight,
        Dead
    }

    /// <summary>
    /// An enumeration of possible player veritcal movement states
    /// </summary>
    enum VerticalMovementState
    {
        OnGround,
        Jumping,
        Falling
    }

    /// <summary>
    /// A class representing the player
    /// </summary>
    public class Player
    {
        // The speed of the walking animation
        const int FRAME_RATE = 300;

        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;

        // The player sprite frames
        Sprite[] frames;

        // The currently rendered frame
        int currentFrame = 0;

        // The player's animation state
        PlayerAnimState animationState = PlayerAnimState.Idle;

        // The player's speed
        int speed = 3;

        // The player's vertical movement state
        VerticalMovementState verticalState = VerticalMovementState.OnGround;

        // A timer for jumping
        TimeSpan jumpTimer;

        // A timer for animations
        TimeSpan animationTimer;

        // The currently applied SpriteEffects
        SpriteEffects spriteEffects = SpriteEffects.None;

        // The color of the sprite
        Color color = Color.White;

        // The origin of the sprite (centered on its feet)
        Vector2 origin = new Vector2(10, 21);

        /// <summary>
        /// Gets and sets the position of the player on-screen
        /// </summary>
        public Vector2 Position = new Vector2(200, 200);

        /// <summary>
        /// Orginal position of player, used for reseting the character when they die
        /// </summary>
        public Vector2 OriginalPosition;

        public BoundingRectangle Bounds => new BoundingRectangle(Position - 1.8f * origin, 38, 41);

        int life = 4;

        bool dead = false;

        ParticleSystem coinParticles;

        public bool Dead
        {
            get
            {
                return dead;
            }
        }

        SoundEffect jumpNoise;

        SoundEffect hurtNoise;

        /// <summary>
        /// Constructs a new player
        /// </summary>
        /// <param name="frames">The sprite frames associated with the player</param>
        /// <param name="x">The spawn point x</param>
        /// <param name="y">The spawn point y</param>
        /// <param name="jumpNoise">The sound effect to play when the player jumps</param>
        /// <param name="hurtNoise">The sound effect to play when the player gets hurt</param>
        /// <param name="jumpParticles">The particle system to activate when the player jumps</param>
        /// <param name="coinParticles">The particle system to activate when the player collects a coin</param>
        public Player(IEnumerable<Sprite> frames, uint x, uint y, SoundEffect jumpNoise, SoundEffect hurtNoise, ParticleSystem coinParticles)
        {
            this.frames = frames.ToArray();
            animationState = PlayerAnimState.WalkingLeft;
            Position.X = x + 10;
            Position.Y = y + 21;
            OriginalPosition = Position;
            this.jumpNoise = jumpNoise;
            this.hurtNoise = hurtNoise;
            this.coinParticles = coinParticles;
        }

        /// <summary>
        /// Updates the player, applying movement and physics
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            // Vertical movement
            switch (verticalState)
            {
                case VerticalMovementState.OnGround:
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        verticalState = VerticalMovementState.Jumping;
                        jumpTimer = new TimeSpan(0);
                        jumpNoise.Play();
                    }
                    break;
                case VerticalMovementState.Jumping:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping with platformer physics
                    Position.Y -= (250 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME) verticalState = VerticalMovementState.Falling;
                    break;
                case VerticalMovementState.Falling:
                    Position.Y += speed;
                    // TODO: This needs to be replaced with collision logic
                    if (Position.Y > 840)
                    {
                        Position.Y = 840;
                        Damage(10);
                    }
                    break;
            }

            if (animationState != PlayerAnimState.Dead)
            {
                // Horizontal movement
                if (keyboard.IsKeyDown(Keys.Left))
                {
                    if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                        animationState = PlayerAnimState.JumpingLeft;
                    else animationState = PlayerAnimState.WalkingLeft;
                    Position.X -= speed;
                }
                else if (keyboard.IsKeyDown(Keys.Right))
                {
                    if (verticalState == VerticalMovementState.Jumping || verticalState == VerticalMovementState.Falling)
                        animationState = PlayerAnimState.JumpingRight;
                    else animationState = PlayerAnimState.WalkingRight;
                    Position.X += speed;
                }
                else
                {
                    animationState = PlayerAnimState.Idle;
                }

                // Apply animations
                switch (animationState)
                {
                    case PlayerAnimState.Idle:
                        currentFrame = 0;
                        animationTimer = new TimeSpan(0);
                        break;

                    case PlayerAnimState.JumpingLeft:
                        spriteEffects = SpriteEffects.FlipHorizontally;
                        currentFrame = 7;
                        break;

                    case PlayerAnimState.JumpingRight:
                        spriteEffects = SpriteEffects.None;
                        currentFrame = 7;
                        break;

                    case PlayerAnimState.WalkingLeft:
                        animationTimer += gameTime.ElapsedGameTime;
                        spriteEffects = SpriteEffects.FlipHorizontally;
                        // Walking frames are 9 & 10
                        if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                        {
                            animationTimer = new TimeSpan(0);
                        }
                        currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE) + 9;
                        break;

                    case PlayerAnimState.WalkingRight:
                        animationTimer += gameTime.ElapsedGameTime;
                        spriteEffects = SpriteEffects.None;
                        // Walking frames are 9 & 10
                        if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                        {
                            animationTimer = new TimeSpan(0);
                        }
                        currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE) + 9;
                        break;

                }

                coinParticles.Update(gameTime);
            }
        }

        public void CheckForPlatformCollision(IEnumerable<IBoundable> platforms)
        {
            Debug.WriteLine($"Checking collisions against {platforms.Count()} platforms");
            if (verticalState != VerticalMovementState.Jumping)
            {
                verticalState = VerticalMovementState.Falling;
                foreach (Platform platform in platforms)
                {
                    if (Bounds.CollidesWith(platform.Bounds))
                    {
                        Position.Y = platform.Bounds.Y - 1;
                        verticalState = VerticalMovementState.OnGround;
                    }
                }
            }
        }

        public int CheckForTokenCollision(IEnumerable<IBoundable> tokens)
        {
            int score = 0;
            Debug.WriteLine($"Checking collisions against {tokens.Count()} tokens");
            foreach (Token token in tokens)
            {
                if (Bounds.CollidesWith(token.Bounds) && token.Active)
                {
                    score++;
                    token.Collect();
                    coinParticles.Emitter = new Vector2(token.Bounds.X + (token.Bounds.Width / 2), token.Bounds.Y + (token.Bounds.Height / 2));
                    coinParticles.SystemLife = 0.5f;
                }
            }
            return score;
        }

        public void Damage(int damage)
        {
            life -= damage;
            hurtNoise.Play();
            if (life <= 0)
            {
                animationState = PlayerAnimState.Dead;
                currentFrame = frames.Length - 1;
                dead = true;
            }
        }

        public void Reset()
        {
            life = 4;
            animationState = PlayerAnimState.Idle;
            verticalState = VerticalMovementState.OnGround;
            spriteEffects = SpriteEffects.None;
            color = Color.White;
            Position = OriginalPosition;
            dead = false;
        }

        /// <summary>
        /// Render the player sprite.  Should be invoked between 
        /// SpriteBatch.Begin() and SpriteBatch.End()
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use</param>
        /// <param name="translationMatrix">Matrix used for translating the particles systems' Draw functions</param>
        public void Draw(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
#if VISUAL_DEBUG 
            VisualDebugging.DrawRectangle(spriteBatch, Bounds, Color.Red);
#endif
            frames[currentFrame].Draw(spriteBatch, Position, color, 0, origin, 2, spriteEffects, 1);
            coinParticles.Draw(translationMatrix);
        }

    }
}
