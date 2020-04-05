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
    /// A non-moving texture rendered using a SpriteBatch
    /// </summary>
    public class StaticSprite : ISprite
    {
        /// <summary>
        /// The sprite's position in game
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The texture of the sprite
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// Creates a new StaticSprite
        /// </summary>
        /// <param name="texture">The texture of the new sprite</param>
        public StaticSprite(Texture2D texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Creates a new StaticSprite
        /// </summary>
        /// <param name="texture">The texture of the new sprite</param>
        /// <param name="position">The position of the new sprite, should be upper-left hand corner</param>
        public StaticSprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        /// <summary>
        /// Draws the sprite using the given SpriteBatch
        /// Should be called between a SpriteBatch.Begin() and a SpriteBatch.End()
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use</param>
        /// <param name="gameTime">The current GameTime</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
