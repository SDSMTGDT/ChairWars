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
using ChairWars.Mobiles;
using ChairWars.DataStructures;
using ChairWars.Json;
using ChairWars.Visual;

namespace ChairWars.Enemies
{
    class Enemy : IsAlive, IInitialize
    {
        public string chairFile;
        public string riderFile;
        public string enemyName { get; private set; }
        public int aiType;
        public List<int> fireTypes;
        public string healthBarFile;

        [Newtonsoft.Json.JsonIgnore]
        public Chair chairUsed;

        [Newtonsoft.Json.JsonIgnore]
        public Chair ChairUsed { get { return chairUsed; } private set { chairUsed = value; } }

        [Newtonsoft.Json.JsonIgnore]
        public int desiredDistance { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool Destroyed { get; set; }
        
        //public Intro intro { get; private set; }
        //public Sprite sprite //for the person in the chair.

        public Enemy()
        {
            fireTypes = new List<int>();
            Destroyed = false;
            desiredDistance = 100;
        }

        public void Initialize()
        {
            JsonExtensions.FromJsonFileAndInit(chairFile, ref chairUsed);

            ChairUsed.SetChairRider(new Sprite(riderFile));

            Globals.collisionManager.AddEnemy(this);
        }

        public void SetCoordinates(Vector2 newCoordinates)
        {
            chairUsed.coordinates = newCoordinates;
        }

        public virtual void Update()
        {
            chairUsed.Update();
        }

        public virtual void Draw()
        {
            chairUsed.Draw();
        }

        public void Move(float rotation/*control stuff*/)
        {
            chairUsed.Move(rotation);
        }

        public void Aim(float rotation/*control stuff*/)
        {
            float temp = 0.0f;
            temp = rotation;
            chairUsed.Aim(temp);
        }
    }
}
