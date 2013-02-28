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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using ChairWars.Mobiles;


namespace ChairWars.Visual
{
    /// <summary>
    /// The camera class will draw the map and follow its target
    /// </summary>
    class Camera
    {
        //Varibale declaration
        private Mobile target;
        public Vector2 coordinates;
        public Vector2 mapOffset;
        private int shake;
        private int shakeDuration;
        private int shakeMaxDuration;
        private bool shakeDecay;
        private bool blackOut;
        private Color tint;
        private int fieldOfVision;
        private float speed;
        private Vector2 targetOffset;
        private float distance;

        private Vector2 upperLeftBounds;
        private Vector2 lowerRightBounds;

        public Vector2 LowerRightBounds
        {
            set { lowerRightBounds = value; }
        }

        /// <summary>
        /// Constructor.  The camera sets all of its default values here.
        /// </summary>
        public Camera()
        {
            coordinates = new Vector2(0.0f, 0.0f);
            mapOffset = new Vector2(0.0f, 0.0f);
            targetOffset = new Vector2(0.0f, 0.0f);
            tint = new Color(new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            shake = 0;
            blackOut = false;
            fieldOfVision = -1;
            speed = 6.0f;
            this.upperLeftBounds = Vector2.Zero;
        }

        /// <summary>
        /// ChangeTarget. Changes the camera's target.
        /// </summary>
        /// <param name="newTarget">The new mobile to be the target.</param>
        public void ChangeTarget(Mobile newTarget)
        {
            target = newTarget;
            speed = newTarget.maxForceMagnitude / 100.0f;
        }

        /// <summary>
        /// ChangeTarget. Changes the camera's target.
        /// </summary>
        /// <param name="newTarget">The new mobile to be the target.</param>
        public void SetShake(int shakeAmount, int duration, bool decay = false)
        {
            shake = shakeAmount;
            shakeDuration = duration;
            shakeMaxDuration = duration;
            shakeDecay = decay;
        }

        /// <summary>
        /// Update.  The camera will update its position based its distance from its current
        /// target. It will also perform shaking, blackouts, and limited view field.
        /// </summary>
        public void Update()
        {
            //find the distance the camera is from the current target
            targetOffset.X = target.coordinates.X - (coordinates.X + 
                (Globals.SCREEN_WIDTH / 2));

            targetOffset.Y = target.coordinates.Y - (coordinates.Y + 
                (Globals.SCREEN_HEIGHT / 2));

            if (this.coordinates.X == this.upperLeftBounds.X)
            {
                if(targetOffset.X < 0)
                    targetOffset.X = 0;
            }
            else if (this.coordinates.X == (this.lowerRightBounds.X - Globals.SCREEN_WIDTH))
            {
                if(targetOffset.X > 0)
                    targetOffset.X = 0;
            }

            if (this.coordinates.Y == this.upperLeftBounds.Y)
            {
                if(targetOffset.Y < 0)
                    targetOffset.Y = 0;
            }
            else if (this.coordinates.Y == (this.lowerRightBounds.Y - Globals.SCREEN_HEIGHT))
            {
                if(targetOffset.Y > 0)
                    targetOffset.Y = 0;
            }
            
            //distance formula
            distance = (float)Math.Sqrt(targetOffset.X * targetOffset.X + targetOffset.Y * targetOffset.Y);

            //if enough distance, then move
            if (distance > speed)
            {
                Move((int)((targetOffset.X / distance) * Math.Abs(targetOffset.X / 10)), (int)((targetOffset.Y / distance) * Math.Abs(targetOffset.Y / 10)));
            }

            //shake mechanics
            if (shake > 0 && shakeDuration > 0)
            {
                coordinates.Y += (float)(Math.Sin(Globals.gameTime.TotalGameTime.TotalMilliseconds) * shake);
                shakeDuration -= Globals.gameTime.ElapsedGameTime.Milliseconds;
                if (shakeDecay)
                {
                    shake = (int)Math.Pow((double)shake, (double)shakeDuration / (double)shakeMaxDuration);
                }
            }
        }

        

        public void Move(int x, int y)
        {
            this.coordinates.X += x;
            this.coordinates.Y += y;

            if (this.coordinates.X > (this.lowerRightBounds.X - Globals.SCREEN_WIDTH))
                this.coordinates.X = this.lowerRightBounds.X - Globals.SCREEN_WIDTH;

            if (this.coordinates.Y > (this.lowerRightBounds.Y - Globals.SCREEN_HEIGHT))
                this.coordinates.Y = this.lowerRightBounds.Y - Globals.SCREEN_HEIGHT;

            if (this.coordinates.X < this.upperLeftBounds.X)
                this.coordinates.X = this.upperLeftBounds.X;

            if (this.coordinates.Y < this.upperLeftBounds.Y)
                this.coordinates.Y = this.upperLeftBounds.Y;

        }

    }
}
