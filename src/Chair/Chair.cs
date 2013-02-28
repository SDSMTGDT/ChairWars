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
using ChairWars.Collisions;
using ChairWars.Weapons;
using ChairWars.Json;
using ChairWars.Visual;

namespace ChairWars.Mobiles
{
    public class Chair : Mobile, IInitialize
    {
        private ChairBase chairBase;
        private ChairBody chairBody;
        private ChairTurret chairTurret;
        private Sprite chairRiderImage;

        public string chairBaseFile { get; set; }
        public string chairBodyFile { get; set; }
        public string chairTurretFile { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public ChairBase ChairBaseAccessor { get { return chairBase; } private set { chairBase = value; } }

        [Newtonsoft.Json.JsonIgnore]
        public ChairBody ChairBodyAccessor { get { return chairBody; } private set { chairBody = value; } }
        
        [Newtonsoft.Json.JsonIgnore]
        public ChairTurret ChairTurretAccessor { get { return chairTurret; } private set { chairTurret = value; } }
        
        [Newtonsoft.Json.JsonIgnore]
        public int maxHP { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int currHP { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int defense { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool alive { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Polygon collisionBox { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float deltaRotation { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float targetAngle { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float currentBodyRotation { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float locomotiveForce { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float locomotiveBoostForce { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int totalMass { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int boostRemaining { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool immobile { get; set; }

        public Chair() : base() { }
        
        public void Initialize()
        {
            immobile = false;
            JsonExtensions.FromJsonFileAndInit(chairBaseFile, ref chairBase);
            JsonExtensions.FromJsonFileAndInit(chairBodyFile, ref chairBody);
            JsonExtensions.FromJsonFileAndInit(chairTurretFile, ref chairTurret);

            defense = chairBase.defense + chairBody.defense + chairTurret.defense;
            maxHP = chairBase.hitPoints + chairBody.hitPoints + chairTurret.hitPoints;
            totalMass = chairBase.mass + chairBody.mass + chairTurret.mass;
            currHP = maxHP;
            alive = true;
            
            collisionBox = new Polygon(chairBody.sprite.GetBoundaryPoints());
            deltaRotation = (float)chairTurret.force / (float)chairBody.mass;
            targetAngle = 0.0f;
            currentBodyRotation = targetAngle;
            locomotiveForce = (float)chairBase.normalForce / (float)totalMass;
            locomotiveBoostForce = (float)chairBase.boostForce / (float)totalMass;
            base.SetAllParameters(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), chairBase.friction, totalMass, chairBase.normalForceLimit, chairBase.boostForceLimit);
            //this.ToJsonFile("testchair.json");
        }

        public void SetChairRider(Sprite newRider)
        {
            chairRiderImage = new Sprite(newRider);
        }
        
        new public void Update()
        {
            float piTimesTwo = (float)Math.PI * 2;
            float tempRotation = (float)Math.Atan2(forceAccumulator.Y, forceAccumulator.X);

            //continue rotation
            if (Math.Abs(currentBodyRotation - targetAngle) > Math.PI && currentBodyRotation - (targetAngle - piTimesTwo) != 0.0)
            {
                if (currentBodyRotation > targetAngle)
                {
                    currentBodyRotation += deltaRotation * -(Math.Abs(currentBodyRotation - (targetAngle + piTimesTwo)) / (currentBodyRotation - (targetAngle + piTimesTwo)));
                }
                else
                {
                    currentBodyRotation += deltaRotation * -(Math.Abs(currentBodyRotation - (targetAngle - piTimesTwo)) / (currentBodyRotation - (targetAngle - piTimesTwo)));
                }
            }
            else if (Math.Abs(currentBodyRotation - targetAngle) > deltaRotation)
            {
                currentBodyRotation += deltaRotation * -(Math.Abs(currentBodyRotation - targetAngle) / (currentBodyRotation - targetAngle));
            }
            else
            {
                currentBodyRotation = targetAngle;
            }
            currentBodyRotation = currentBodyRotation % piTimesTwo;
            if (currentBodyRotation < 0.0f)
            {
                currentBodyRotation += (float)piTimesTwo;
            }

            if (!immobile)
            {
                base.Update();
            }
            chairBody.Update(coordinates, currentBodyRotation);
            chairBase.Update(coordinates, tempRotation);
            collisionBox.MoveTo(coordinates);
            collisionBox.Rotate(currentBodyRotation);

            if (chairRiderImage != null)
            {
                chairRiderImage.coordinates = coordinates;
                chairRiderImage.Rotation = currentBodyRotation;
            }
            
        }

        public void Draw()
        {
            chairBase.Draw();
            chairBody.Draw();
            if (chairRiderImage != null)
            {
                chairRiderImage.Draw();
            }
        }

        public void Reload(int fireType)
        {
            chairBody.Reload(fireType);
        }

        public void Fire(int fireType)
        {
            chairBody.Fire(fireType);
        }

        public void Fire(List<int> fireType)
        {
            int i, iMax;
            iMax = fireType.Count;
            for (i = 0; i < iMax; i++)
            {
                chairBody.Fire(fireType[i]);
            }
        }

        public void Aim(float newTarget)
        {
            targetAngle = newTarget;
            if (targetAngle < 0.0f)
            {
                targetAngle += (float)Math.PI * 2;
            }
        }

        public void Move(float rotation/*control stuff*/)
        {
            Vector2 tempVector = new Vector2(0.0f, 0.0f);
            
            tempVector.X = (float)(locomotiveForce * Math.Cos(rotation));
            tempVector.Y = (float)(locomotiveForce * Math.Sin(rotation));
            AddNormalForce(tempVector);
        }

        public void Boost(float rotation)
        {
            if(boostRemaining > 0 )
            {
                Vector2 tempVector = new Vector2(0.0f, 0.0f);
                tempVector.X = (float)(locomotiveBoostForce * Math.Cos(rotation));
                tempVector.Y = (float)(locomotiveBoostForce * Math.Sin(rotation));
                AddBoostForce(tempVector);
                boostRemaining--;
            }
        }

        public void GetDamaged(IProjectile projectile)
        {
            currHP = currHP - (projectile.ProjectileDamage - defense);
            //knockback. add it.
            if (currHP <= 0)
            {
                alive = false;
            }
            if (alive == false)
            {
                //replace this with the signal version in the furtue.
                Globals.mobileManager.RemoveEnemy(this);
                //send signal
                //call explosion
                Globals.particleEngine.CreateParticles(chairBody.deathExplosionFile, this);
                Globals.camera.SetShake(7, 500);
            }
            
        }

        public void HandleCollision(Vector2 temp, float kineticEnergy)
        {
            if (!immobile)
            {
                Collide(temp, kineticEnergy, totalMass);
                collisionBox.Move(temp);
            }
        }

        
    }
}
