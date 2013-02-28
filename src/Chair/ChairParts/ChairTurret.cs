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
using System.Xml.Linq;
using ChairWars.Json;

namespace ChairWars.Mobiles
{
    public class ChairTurret : IInitialize
    {
        public int mass;
        public int force;
        public int weightLimit;
        public int hitPoints;
        public int defense;

        public ChairTurret() { }

        public void Initialize()
        {
            
        }

    }
}
