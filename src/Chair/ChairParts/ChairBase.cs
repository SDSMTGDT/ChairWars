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
using ChairWars.Visual;
using ChairWars.Json;

namespace ChairWars.Mobiles
{
    public class ChairBase : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        public Sprite sprite;
        public string spriteFile;

        public int mass;
        public int normalForce;
        public int normalForceLimit;
        public int boostLimit;
        public int boostForce;
        public int boostForceLimit;
        public int supportMass;
        public float rotation;
        public int hitPoints;
        public int defense;
        public float friction;

        public ChairBase()
        {

        }

        public void Update(Vector2 newCoordinates, float newRotation)
        {
            rotation = newRotation;
            sprite.Update(newCoordinates, rotation);
        }

        public void Draw()
        {
            sprite.Draw();
        }

        public void Initialize()
        {
            sprite = new Sprite(spriteFile);
        }
    }
}
