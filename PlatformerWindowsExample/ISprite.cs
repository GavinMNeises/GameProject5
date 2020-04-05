using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PlatformerExample
{
    /// <summary>
    /// Interface representing a sprite to be drawn using a SpriteBatch
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// Draws the ISprite
        /// Should be called between a SpriteBatch.Begin() and a SpriteBatch.End()
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use</param>
        /// <param name="gameTime">The current GameTime</param>
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
