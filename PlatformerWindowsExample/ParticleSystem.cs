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
    /// A delegate for spawning particles
    /// </summary>
    /// <param name="particle">The particle to spawn</param>
    public delegate void ParticleSpawner(ref Particle particle);

    /// <summary>
    /// A delegate for updating particles
    /// </summary>
    /// <param name="deltaT">The seconds elapsed between frames</param>
    /// <param name="particle">The particle to update</param>
    public delegate void ParticleUpdater(float deltaT, ref Particle particle);

    public class ParticleSystem
    {
        /// <summary>
        /// The collection of particles
        /// </summary>
        Particle[] particles;

        /// <summary>
        /// The texture of the particles
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The SpriteBatch used to render the particles
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Used to get random numbers
        /// </summary>
        Random random = new Random();

        /// <summary>
        /// The central location of the particles
        /// </summary>
        public Vector2 Emitter { get; set; }

        /// <summary>
        /// The rate at which particles will spawn
        /// </summary>
        public int SpawnPerFrame { get; set; }

        /// <summary>
        /// Keeps track of the next free index in the particle array
        /// </summary>
        int nextIndex = 0;

        /// <summary>
        /// Holds a delegate to use when spawning a new particle
        /// </summary>
        public ParticleSpawner SpawnParticle { get; set; }

        /// <summary>
        /// Holds a delegate to use when updating a particle 
        /// </summary>
        public ParticleUpdater UpdateParticle { get; set; }

        public float SystemLife { get; set; }

        /// <summary>
        /// This initializes a new ParticleSystem
        /// </summary>
        /// <param name="graphicsDevice">Used to make the SpriteBatch</param>
        /// <param name="size">Size of the Particle array</param>
        /// <param name="texture">Texture of the particles</param>
        public ParticleSystem(GraphicsDevice graphicsDevice, int size, Texture2D texture)
        {
            this.particles = new Particle[size];
            this.spriteBatch = new SpriteBatch(graphicsDevice);
            this.texture = texture;
            SystemLife = 0;
        }

        /// <summary>
        /// Creates new particles for the system then
        /// it updates all particles in the system
        /// </summary>
        /// <param name="gameTime">Used to move the particles consistently with time passed</param>
        public void Update(GameTime gameTime)
        {
            //Make sure our delegate properties are set
            if (SpawnParticle == null || UpdateParticle == null) return;

            //Spawn Particles if system still has life
            if (SystemLife > 0)
            {
                for (int i = 0; i < SpawnPerFrame; i++)
                {
                    //Create the particle
                    SpawnParticle(ref particles[nextIndex]);

                    //Advance nextIndex
                    nextIndex++;
                    if (nextIndex > particles.Length - 1) nextIndex = 0;
                }
            }

            //Update Particles
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < particles.Length; i++)
            {
                //Skip dead particles
                if (particles[i].Life <= 0) continue;

                //Update live particles
                UpdateParticle(deltaT, ref particles[i]);
            }

            SystemLife -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Draws the active particles in the system
        /// </summary>
        public void Draw(Matrix translationMatrix)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, translationMatrix);

            for (int i = 0; i < particles.Length; i++)
            {
                //Skip dead particles
                if (particles[i].Life <= 0) continue;

                //Draw the individual particle
                spriteBatch.Draw(texture, particles[i].Position, null, particles[i].Color, 0f, Vector2.Zero, particles[i].Scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }
    }
}
