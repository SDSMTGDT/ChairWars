using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ChairWars.Visual;

namespace ChairWars.Arena
{
    class ArenaMap
    {
        private Sprite mapSprite;
        private Obstacle[] walls = new Obstacle[4];
        //music ambientSounds[10];
        public int id { get; private set; }
        public string name { get; private set; }

        public ArenaMap ( string file )
        {
            
        }

        public void Update ()
        {

        }

        public void Draw ()
        {

        }

    }
}
