using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlatformerExample
{
    /// <summary>
    /// A controller that scrolls a parallax layer at a set speed
    /// </summary>
    public class AutoScrollController : IScrollController
    {
        /// <summary>
        /// The time that has elapsed
        /// </summary>
        float elapsedTime = 0;

        /// <summary>
        /// The speed the layer should scroll at
        /// </summary>
        public float Speed = 10f;

        /// <summary>
        /// Returns the current transformation matrix
        /// </summary>
        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(-elapsedTime * Speed, 0, 0);
            }
        }

        /// <summary>
        /// Creates a new Auto Scrolling Controller
        /// </summary>
        /// <param name="speed">The speed at which the layer scrolls at</param>
        public AutoScrollController(float speed)
        {
            Speed = speed;
        }

        /// <summary>
        /// Updates the elapsedTime of this controller
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
