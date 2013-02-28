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

namespace ChairWars.Arena
{
    class Map : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        private Sprite mapSprite;
        
        [Newtonsoft.Json.JsonIgnore]
        private List<Obstacle> walls;

        [Newtonsoft.Json.JsonIgnore]
        public Vector2 mapBounds;

        public string spriteFile;
        public string name;
        public Vector2 playerStartLocation;
        public Vector2 enemyStartLocation;
        //private string wallFile;
        //private List<Sounds> ambientSounds;
        
        public Map(/*string fileName*/)
        {
            walls = new List<Obstacle>();
            //mapSprite = new Sprite(spriteFile);
            
        }

        public void Initialize()
        {
            mapSprite = new Sprite(spriteFile);

            mapBounds = new Vector2(mapSprite.image.Width, mapSprite.image.Height);

            float maxWallThickness = 500.0f;
            float wallThickness = 50.0f;
            mapSprite.coordinates = new Vector2(mapSprite.image.Width / 2.0f, mapSprite.image.Height / 2.0f);
            //vertical walls
            walls.Add(new Obstacle(new Vector2(wallThickness - (maxWallThickness / 2), mapSprite.image.Height / 2.0f), 0.0f, maxWallThickness, mapSprite.image.Height));
            walls.Add(new Obstacle(new Vector2(mapSprite.image.Width + (maxWallThickness / 2) - wallThickness, mapSprite.image.Height / 2.0f), 0.0f, maxWallThickness, mapSprite.image.Height));
            
            //horizontal walls
            walls.Add(new Obstacle(new Vector2(mapSprite.image.Width / 2.0f, -(maxWallThickness / 2) + wallThickness), 0.0f, mapSprite.image.Width, maxWallThickness));
            walls.Add(new Obstacle(new Vector2(mapSprite.image.Width / 2.0f, mapSprite.image.Height + (maxWallThickness / 2) - wallThickness), 0.0f, mapSprite.image.Width, maxWallThickness));
            
            Globals.camera.LowerRightBounds = new Vector2(mapSprite.image.Width, mapSprite.image.Height);
        }

        public void Update()
        {
            //ambient sound stuff
        }

        public void Draw()
        {
            mapSprite.Draw();
        }

    }
}
 