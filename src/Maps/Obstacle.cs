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
using ChairWars.Collisions;
using ChairWars.DataStructures;

namespace ChairWars.Arena
{
    class Obstacle : IsAlive
    {
        //private Sprite obstacleSprite;
        public Polygon collisionBox;
        public Vector2 coordinates { get; private set; }
        public float rotation { get; private set; }
        public bool Destroyed { get; set; }

        public Obstacle ( /*string fileName, */Vector2 coords, float rotate, float width, float height )
        {
            
            //obstacleSprite = new Sprite(fileName);

            //collisionBox = new Polygon(obstacleSprite.GetBoundaryPoints());
            List<Vector2> tempList = new List<Vector2>();
            tempList.Add(new Vector2(0.0f, 0.0f));
            tempList.Add(new Vector2(width, 0.0f));
            tempList.Add(new Vector2(width, height));
            tempList.Add(new Vector2(0.0f, height));

            collisionBox = new Polygon(tempList);

            coordinates = coords;
            rotation = rotate;

            collisionBox.MoveTo(coordinates);
            collisionBox.Rotate(rotation);

            Globals.collisionManager.AddObstacle(this);

            //obstacleSprite.Update(coordinates, rotation);
        }

        public void Draw()
        {
            //obstacleSprite.Draw();
        }
    }
}
