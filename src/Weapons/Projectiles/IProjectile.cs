using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChairWars.Visual;
using ChairWars.Collisions;
using ChairWars.DataStructures;

namespace ChairWars.Weapons
{
    public interface IProjectile:IsAlive
    {
        Sprite ProjectileSprite
        {
            get;
            set;
        }

        int ProjectileMass
        {
            get;
            set;
        }

        int InvincibleFrames
        {
            get;
            set;
        }

        int Lifespan
        {
            get;
            set;
        }

        Polygon CollisionBox
        {
            get;
            set;
        }

        int ProjectileDamage
        {
            get;
            set;
        }

        int InitialVelocity
        {
            get;
            set;
        }

        int OnDeathAttackType
        {
            get;
            set;
        }

        List<AttackFile> OnDeathFiles
        {
            get;
            set;
        }

        string ParticleTrailFile
        {
            get;
            set;
        }

        string ParticleFile
        {
            get;
            set;
        }

        string SoundCueOnDeath
        {
            get;
            set;
        }

        void Update();
        void InitializeProjectile(Vector2 position, float rotation, float rotationOffset);
        List<IProjectile> OnDeath();
    }
}
