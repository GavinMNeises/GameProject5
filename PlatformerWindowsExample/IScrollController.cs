using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlatformerExample
{
    /// <summary>
    /// An interface for a parallax scrolling controller
    /// </summary>
    public interface IScrollController
    {
        /// <summary>
        /// The current transformation matrix to use
        /// </summary>
        Matrix Transformation { get; }

        /// <summary>
        /// Updates the transformation matrix
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        void Update(GameTime gameTime);
    }
}
