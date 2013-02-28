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

namespace ChairWars.Visual
{
    public class Sprite : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        public Texture2D image { get; set; }
        public string imageFile;
        public Vector2 coordinates { get; set; }
        public Color spriteColor;

        private float rotation;
        public bool flipVertical;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public Vector2 spriteOrigin { get; set; }

        public Sprite(string fileName)
        {
            string[] fileParts;
            
            if (fileName == null || fileName == "")
            {
                return;
            }
            fileParts = fileName.Split('.');
            imageFile = fileParts[0];
            //System.IO.FileStream stream = System.IO.File.OpenRead(".\\Content\\" + imageFile);
            //image = Texture2D.FromStream(Globals.graphicsDeviceManager.GraphicsDevice, stream);
            image = Globals.contentManager.Load<Texture2D>(".\\Content\\" + imageFile);
            spriteOrigin = new Vector2(image.Width / 2.0f, image.Height / 2.0f);
            spriteColor = Color.White;
        }

        public Sprite()
        {
            
        }

        public Sprite(Sprite source)
        {
            image = source.image;
            spriteOrigin = source.spriteOrigin;
            spriteColor = source.spriteColor;
        }

        public void SetFlip(bool flip)
        {
            flipVertical = flip;
        }

        public void Update(Vector2 newCoordinates, float newRotation)
        {
            rotation = newRotation;

            coordinates = newCoordinates;
        }

        public void Draw()
        {
            if (coordinates.X - Globals.camera.coordinates.X + image.Width > 0 
                && coordinates.X - Globals.camera.coordinates.X - image.Width < Globals.SCREEN_WIDTH)
            {
                if (coordinates.Y - Globals.camera.coordinates.Y + image.Height > 0
                    && coordinates.Y - Globals.camera.coordinates.Y - image.Height < Globals.SCREEN_HEIGHT)
                {
                    if (flipVertical)
                    {
                        Globals.spriteBatch.Draw(image, coordinates - Globals.camera.coordinates, null, spriteColor, rotation, spriteOrigin, 1.0f, SpriteEffects.FlipVertically, 0.0f);
                    }
                    else
                    {
                        Globals.spriteBatch.Draw(image, coordinates - Globals.camera.coordinates, null, spriteColor, rotation, spriteOrigin, 1.0f, SpriteEffects.None, 0.0f);
                    }
                }
            }
        }

        public void Draw(Vector2 offset, Rectangle rect)
        {
            Globals.spriteBatch.Draw(image, coordinates + offset, rect, Color.White);
        }

        public List<Vector2> GetBoundaryPoints()
        {
            List<Vector2> tempList = new List<Vector2>();
            
            tempList.Add(new Vector2(image.Width / 2.0f, image.Height / 2.0f));
            tempList.Add(new Vector2(image.Width / 2.0f, -image.Height / 2.0f));
            tempList.Add(new Vector2(-image.Width / 2.0f, -image.Height / 2.0f));
            tempList.Add(new Vector2(-image.Width / 2.0f, image.Height / 2.0f));

            return tempList;
        }


        public void Initialize()
        {
            System.IO.FileStream stream = System.IO.File.OpenRead(".\\Content\\" + imageFile);
            image = Texture2D.FromStream(Globals.graphicsDeviceManager.GraphicsDevice, stream);
            //image = Globals.contentManager.Load<Texture2D>(fileName);
            spriteOrigin = new Vector2(image.Width / 2.0f, image.Height / 2.0f);
            spriteColor = Color.White;
        }
    }
}
