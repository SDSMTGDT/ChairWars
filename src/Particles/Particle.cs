using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Mobiles;
using ChairWars.Visual;
using ChairWars.DataStructures;
using Microsoft.Xna.Framework;

namespace ChairWars.Particles
{
    class Particle : Mobile, IsAlive
    {
        public Sprite sprite;
        public int lifeTime;
        private bool destroyed;
        public bool Destroyed
        {
            get
            {
                return destroyed;
            }
            set
            {
                destroyed = value;
            }
        }

        public Particle(Mobile motion, Sprite sp, int lifeSpan)
        {
            sprite = sp;
            lifeTime = lifeSpan;
            base.SetAllParameters(motion);
            sprite.coordinates = base.coordinates;
            Destroyed = false;
        }

        public void Draw()
        {
            sprite.Draw();
        }

        public void Update()
        {
            lifeTime -= Globals.gameTime.ElapsedGameTime.Milliseconds;
            if (lifeTime <= 0)
            {
                Destroyed = true;
            }
            base.Update();
            sprite.coordinates = base.coordinates;
        }

        
    }
}
