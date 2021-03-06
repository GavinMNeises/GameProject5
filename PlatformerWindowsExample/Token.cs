﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PlatformerExample
{
    public class Token : IBoundable
    {
        /// <summary>
        /// The token's bounds
        /// </summary>
        BoundingRectangle bounds;

        /// <summary>
        /// The token's sprite
        /// </summary>
        Sprite sprite;

        /// <summary>
        /// The bounding rectangle of the token
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        bool active;

        public bool Active => active;

        SoundEffect pickupSound;

        /// <summary>
        /// Constructs a new token
        /// </summary>
        /// <param name="bounds">The token's bounds</param>
        /// <param name="sprite">The token's sprite</param>
        public Token(BoundingRectangle bounds, Sprite sprite, SoundEffect pickupSound)
        {
            this.bounds = bounds;
            this.sprite = sprite;
            active = true;
            this.pickupSound = pickupSound;
        }

        public void Collect()
        {
            active = false;
            pickupSound.Play();
        }

        public void Reset()
        {
            active = true;
        }

        /// <summary>
        /// Draws the platform
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch to render to</param>
        public void Draw(SpriteBatch spriteBatch)
        {
#if VISUAL_DEBUG
                VisualDebugging.DrawRectangle(spriteBatch, bounds, Color.Green);
#endif
            if (active) sprite.Draw(spriteBatch, bounds, Color.White);
        }
    }
}
