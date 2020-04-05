using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerExample
{
    /// <summary>
    /// Represents a single parallax layer
    /// </summary>
    public class ParallaxLayer : DrawableGameComponent
    {
        /// <summary>
        /// List of ISprites to represent the parallax layer contents
        /// </summary>
        public List<ISprite> Sprites = new List<ISprite>();

        /// <summary>
        /// Used to draw the parallax layer
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// The controller for this layer
        /// </summary>
        public IScrollController ScrollController { get; set; }

        /// <summary>
        /// Creates a new Player Tracking Parallax Layer
        /// </summary>
        /// <param name="game">The game the layer belongs to</param>
        /// <param name="player">The player to track</param>
        /// <param name="ratio">The ratio of the layer</param>
        public ParallaxLayer(Game game, Player player, float ratio) : base(game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            ScrollController = new PlayerTrackingScrollController(player, ratio);
        }

        /// <summary>
        /// Creates a new Auto Scrolling Parallax Layer
        /// </summary>
        /// <param name="speed">The speed of the layer</param>
        /// <param name="game">The game the layer belongs to</param>
        public ParallaxLayer(Game game, float speed) : base(game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            ScrollController = new AutoScrollController(speed);
        }

        /// <summary>
        /// Updates the parallax layer
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public override void Update(GameTime gameTime)
        {
            ScrollController.Update(gameTime);
        }

        /// <summary>
        /// Draws the parallax layer
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ScrollController.Transformation);
            foreach (var sprite in Sprites)
            {
                sprite.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }
    }
}
