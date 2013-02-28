using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChairWars.Visual;
using ChairWars.Json;
using ChairWars.Mobiles;

namespace ChairWars.Particles
{
    class ParticleSource : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        public List<Sprite> sprite;

        public List<string> spriteFile;
        public List<Vector4> spriteColors;
        public Vector2 positionOffset;
        public Vector2 positionVariance; //x and y maximum
        public int speedAverage;
        public float speedVariance; //1.0 = 100%
        public float angleOffset;
        public float angleVariance; //in radians pi is max
        public int numberOfParticles;
        public int numberOfExplosions; //used for multiple, similar explosions
        public float frictionAverage;
        public float frictionVariance; //1.0 = 100%
        public int lifetimeAverage;
        public float lifetimeVariance;

        public ParticleSource()
        {
            sprite = new List<Sprite>();
            spriteFile = new List<string>();
            spriteColors = new List<Vector4>();
        }

        public void Initialize()
        {

        }

        public List<Particle> MakeParticles(Mobile source)
        {
            int i, j;
            int iMax;
            List<Particle> returnList = new List<Particle>();
            Sprite tempSprite;

            iMax = spriteFile.Count;
            for (i = 0; i < iMax; i++)
            {
                sprite.Add(new Sprite(spriteFile[i]));
            }
            Mobile tempMobile = new Mobile();
            Vector2 tempPosition = new Vector2();
            Vector2 tempForce = new Vector2();
            Vector4 tempVector = new Vector4();
            int tempSpeed;
            int tempLifetime;
            double tempDirection;
            float tempFriction;
            float randomNumber;

            for(i = 0; i < numberOfExplosions; i++)
            {
                //Set explosion coordinates
                randomNumber = Globals.randomNumberGenerator.Next((int)-positionVariance.X, (int)positionVariance.X);
                tempPosition.X = source.coordinates.X + positionOffset.X + randomNumber;
                randomNumber = Globals.randomNumberGenerator.Next((int)-positionVariance.Y, (int)positionVariance.Y);
                tempPosition.Y = source.coordinates.Y + positionOffset.Y + randomNumber;
                
                for(j = 0; j < numberOfParticles; j++)
                {
                    randomNumber = (float)Globals.randomNumberGenerator.Next(-100, 100);
                    randomNumber /= 100.0f;

                    tempSpeed = (int)(randomNumber * speedAverage * speedVariance);
                    tempSpeed += speedAverage;

                    tempLifetime = (int)(randomNumber * lifetimeVariance * lifetimeAverage);
                    tempLifetime += lifetimeAverage;

                    randomNumber = (float)Globals.randomNumberGenerator.Next(-100, 100);
                    randomNumber /= 100.0f;

                    tempDirection = Math.Atan2((double)source.forceAccumulator.Y, (double)source.forceAccumulator.X);
                    tempDirection += randomNumber * angleVariance;
                    tempDirection += angleOffset;

                    tempFriction = randomNumber * frictionVariance * frictionAverage;
                    tempFriction += frictionAverage;

                    tempForce.X = tempSpeed * (float)Math.Cos(tempDirection);
                    tempForce.Y = tempSpeed * (float)Math.Sin(tempDirection);

                    tempMobile.SetAllParameters(tempPosition, tempForce, tempFriction, 1, int.MaxValue, int.MaxValue);
                    
                    randomNumber = Globals.randomNumberGenerator.Next(0, sprite.Count);
                    tempSprite = new Sprite(sprite[(int)randomNumber]);
                    randomNumber = Globals.randomNumberGenerator.Next(0, spriteColors.Count);
                    tempVector = spriteColors[(int)randomNumber];
                    tempSprite.spriteColor.A = (byte)tempVector.W;
                    tempSprite.spriteColor.R = (byte)tempVector.X;
                    tempSprite.spriteColor.G = (byte)tempVector.Y;
                    tempSprite.spriteColor.B = (byte)tempVector.Z;
                    //tempSprite.spriteColor = new Color(tempVector);
                    returnList.Add(new Particle(tempMobile, tempSprite, tempLifetime));
                }
            }

            return returnList;
        }
    }
}
