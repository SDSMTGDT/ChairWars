using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Json;
using ChairWars.DataStructures;
using ChairWars.Mobiles;
using ChairWars.Visual;
using Microsoft.Xna.Framework;

namespace ChairWars.Particles
{
    class ParticleEngine
    {
        public LazyList<Particle> particles;
        public List<Particle> particlesToAdd;
        public ParticleSource particleSource;

        public ParticleEngine()
        {
            particles = new LazyList<Particle>();
            particlesToAdd = new List<Particle>();
        }

        public void CreateParticles(string fileName, Mobile source)
        {
            int i;
            JsonExtensions.FromJsonFileAndInit(fileName, ref particleSource);
            particlesToAdd = particleSource.MakeParticles(source);
            for (i = 0; i < particlesToAdd.Count; i++)
            {
                particles.Add(particlesToAdd[i]);
            }
            particlesToAdd.Clear();
        }

        public void Update()
        {
            int iMax = particles.Alive;
 
            for (int i = 0; i < iMax; i++)
            {
                particles[i].Update();
                if (particles[i].Destroyed == true)
                {
                    particles.RemoveAt(i);
                }
            }
            particles.ClearDead();
        }

        public void Draw()
        {
            int iMax = particles.Alive;
            if (iMax > 3000)
            {
                iMax = 3000;
            }
            for (int i = 0; i < iMax; i++)
            {
                particles[i].Draw();
            }
        }
    }
}
