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

namespace ChairWars.Mobiles
{
    public class Mobile
    {
        public Vector2 coordinates;
        public Vector2 forceAccumulator;
        public float friction { get; private set; }
        public int mass { get; private set; }
        public int maxForceMagnitude { get; private set; }
        public int maxBoostForceMagnitude { get; private set; }


        public Mobile()
        {
            SetAllParameters(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), 0.0f, 1, 1, 1);
        }

        public Mobile(Vector2 coord, Vector2 initForce, float fric, int m, int maxForce, int maxBoost)
        {
            SetAllParameters(coord, initForce, fric, m, maxForce, maxBoost);
        }

        public void SetAllParameters(Vector2 coord, Vector2 initForce, float fric, int m, int maxForce, int maxBoost)
        {
            coordinates = coord;
            forceAccumulator = initForce;
            friction = fric;
            mass = m;
            maxForceMagnitude = maxForce;
            maxBoostForceMagnitude = maxBoost;

        }

        public void SetAllParameters(Mobile copyMobile)
        {
            SetAllParameters(copyMobile.coordinates, copyMobile.forceAccumulator, copyMobile.friction, copyMobile.mass, copyMobile.maxForceMagnitude, copyMobile.maxBoostForceMagnitude);
        }

        public void Update()
        {
            uint timeDifference;
            float fractionalSecond;

            //calculate time difference
            timeDifference = (uint)Globals.gameTime.ElapsedGameTime.TotalMilliseconds;
            fractionalSecond = timeDifference / 1000.0f;

            //update coordinates
            coordinates.X += fractionalSecond * forceAccumulator.X;
            coordinates.Y += fractionalSecond * forceAccumulator.Y;

            //update force accumulator
            forceAccumulator.X -= fractionalSecond * friction * forceAccumulator.X;
            forceAccumulator.Y -= fractionalSecond * friction * forceAccumulator.Y;
        }

        public void AddNormalForce(Vector2 force)
        {
            //if already going faster than normal, do nothing.
            if ((int)forceAccumulator.LengthSquared() > maxForceMagnitude * maxForceMagnitude)
            {
                return;
            }

            forceAccumulator += force;

            //if the new force is higher than the allowed max, restrict to max
            if ((int)forceAccumulator.LengthSquared() > maxForceMagnitude * maxForceMagnitude)
            {
                forceAccumulator.Normalize();
                forceAccumulator.X *= maxForceMagnitude;
                forceAccumulator.Y *= maxForceMagnitude;
            }

        }

        public void AddBoostForce(Vector2 force)
        {
            //if already going faster than normal, do nothing.
            if ((int)forceAccumulator.LengthSquared() > maxBoostForceMagnitude * maxBoostForceMagnitude)
            {
                return;
            }

            forceAccumulator += force;

            //if the new force is higher than the allowed max, restrict to max
            if ((int)forceAccumulator.LengthSquared() > maxBoostForceMagnitude * maxBoostForceMagnitude)
            {
                forceAccumulator.Normalize();
                forceAccumulator.X *= maxBoostForceMagnitude;
                forceAccumulator.Y *= maxBoostForceMagnitude;
            }

        }

        public void Collide(Vector2 separationVector, float kineticEnergy, int mass)
        {
            coordinates.X += separationVector.X;
            coordinates.Y += separationVector.Y;
            separationVector.Normalize();
            kineticEnergy /= mass;
            if (Math.Abs(separationVector.X) > 0.001f)
            {
                forceAccumulator.X += separationVector.X * kineticEnergy;
            }
            if (Math.Abs(separationVector.Y) > 0.001f)
            {
                forceAccumulator.Y += separationVector.Y * kineticEnergy;
            }
        }


        public void AddUncheckedForce(Vector2 force)
        {
            forceAccumulator += force;
        }

    }
}
