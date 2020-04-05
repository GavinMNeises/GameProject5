using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlatformerExample
{
    /// <summary>
    /// A parallax scroll controller that follows the player's movement
    /// </summary>
    public class PlayerTrackingScrollController : IScrollController
    {
        /// <summary>
        /// The player to track
        /// </summary>
        Player player;

        /// <summary>
        /// How fast the layer should scroll relative to the player
        /// Must be between 0 and 1
        /// </summary>
        public float ScrollRatio = 1.0f;

        /// <summary>
        /// The offset between the layer and the player
        /// </summary>
        public float Offset = 200;

        /// <summary>
        /// Gets the current transformation of the layer
        /// </summary>
        public Matrix Transformation
        {
            get
            {
                float x = ScrollRatio * (Offset - player.Position.X);
                float y = ScrollRatio * ScrollRatio * (Offset - player.Position.Y);
                return Matrix.CreateTranslation(x, y, 0);
            }
        }

        /// <summary>
        /// Updates the controller
        /// no-op
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public void Update(GameTime gameTime) { }

        /// <summary>
        /// Creates the new player tracking scroll controller
        /// </summary>
        /// <param name="player">The player to track</param>
        /// <param name="ratio">The ratio of the layer, should be between 0 and 1</param>
        public PlayerTrackingScrollController(Player player, float ratio)
        {
            this.player = player;
            ScrollRatio = ratio;
        }
    }
}
