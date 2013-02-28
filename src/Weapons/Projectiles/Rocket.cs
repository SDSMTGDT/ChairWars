using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using ChairWars.Mobiles;
using ChairWars.Visual;
using ChairWars.Collisions;
using ChairWars.Json;

namespace ChairWars.Weapons
{
    public class Rocket : Mobile, IProjectile
    {
        #region Rocket Members

        public int RocketHealth {get; set;}

        #endregion

        #region IProjectile Members

        private Sprite _projectileSprite;
        public Sprite ProjectileSprite
        {
            get
            {
                return _projectileSprite;
            }
            set
            {
            }
        }

        private int _lifespan;
        public int Lifespan
        {
            get
            {
                return _lifespan;
            }
            set
            {
                _lifespan = value;
            }
        }

        private string _spriteFile;
        public string SpriteFile
        {
            get
            {
                return _spriteFile;
            }
            set
            {
                _spriteFile = value;
            }
        }

        private int _projectileDamage;
        public int ProjectileDamage
        {
            get
            {
                return _projectileDamage;
            }
            set
            {
                _projectileDamage = value;
            }
        }

        private int projectileMass;
        public int ProjectileMass
        {
            get
            {
                return projectileMass;
            }
            set
            {
                projectileMass = value;
            }
        }

        private int invincibleFrames;
        public int InvincibleFrames
        {
            get
            {
                return invincibleFrames;
            }
            set
            {
                invincibleFrames = value;
            }
        }

        private Polygon _collisionBox;
        public Polygon CollisionBox
        {
            get
            {
                return _collisionBox;
            }
            set
            {
                _collisionBox = value;
            }
        }

        private int initalVelocity;
        public int InitialVelocity
        {
            get
            {
                return initalVelocity;
            }
            set
            {
                initalVelocity = value;
            }
        }

        private int onDeathAttackType;
        public int OnDeathAttackType
        {
            get
            {
                return onDeathAttackType;
            }
            set
            {
                onDeathAttackType = value;
            }
        }

        private List<AttackFile> onDeathFiles;
        public List<AttackFile> OnDeathFiles
        {
            get
            {
                return onDeathFiles;
            }
            set
            {
                onDeathFiles = value;
            }
        }
        private string _particleTrailFile;
        public string ParticleTrailFile
        {
            get
            {
                return _particleTrailFile;
            }
            set
            {
                _particleTrailFile = value;
            }
        }

        private string _particleFile;
        public string ParticleFile
        {
            get
            {
                return _particleFile;
            }
            set
            {
                _particleFile = value;
            }
        }

        private string _soundCueOnDeath;
        public string SoundCueOnDeath
        {
            get
            {
                return _soundCueOnDeath;
            }
            set
            {
                _soundCueOnDeath = value;
            }
        }

        #endregion

        #region IsAlive Members

        private bool _destroyed;
        public bool Destroyed
        {
            get
            {
                return _destroyed;
            }
            set
            {
                _destroyed = value;
            }
        }

        #endregion

        public Rocket()
        {
        }


        public void InitializeProjectile(Vector2 position, float rotation, float rotationOffset)
        {
            if (rotationOffset == -1)
            {

                rotationOffset = (float)(Globals.randomNumberGenerator.Next(1000) * Math.PI / 500);
            }

            _projectileSprite = new Sprite(SpriteFile);
            _projectileSprite.Update(position, rotation + rotationOffset);
            _collisionBox = new Polygon(_projectileSprite.GetBoundaryPoints());

            Vector2 force = new Vector2((float)Math.Cos(rotation + rotationOffset) * InitialVelocity,
                                        (float)Math.Sin(rotation + rotationOffset) * InitialVelocity);
            base.SetAllParameters(position, force, 0, projectileMass, int.MaxValue, int.MaxValue);
        }


        public new void Update()
        {
            if (InvincibleFrames != 0)
                InvincibleFrames--;

            base.Update();
            _projectileSprite.Update(base.coordinates, _projectileSprite.Rotation);
            _collisionBox.MoveTo(base.coordinates);
            _collisionBox.Rotate(_projectileSprite.Rotation);

            if (_particleTrailFile != null)
            {
                Globals.particleEngine.CreateParticles(_particleTrailFile, this);
            }

            if (Lifespan > 0)
            {
                Lifespan--;
                if (Lifespan == 0)
                {
                    OnDeath();
                }
            }
        }

        public List<IProjectile> OnDeath()
        {
            List<IProjectile> projectiles = new List<IProjectile>();

            //I'm gonna live forever!
            _destroyed = true;

            if (Lifespan == 0)
                return projectiles;

            if (!string.IsNullOrEmpty(_soundCueOnDeath))
            {
                Globals.soundBankSoundEffects.PlayCue(_soundCueOnDeath);
            }
            
            if (_particleFile != null)
            {
                Globals.particleEngine.CreateParticles(_particleFile, this);
            }

            Bullet newBullet = null;
            Rocket newRocket = null;
            Melee newMelee = null;

            Vector2 position = _projectileSprite.coordinates;
            float rotation = _projectileSprite.Rotation;

            foreach (AttackFile AttackFile in OnDeathFiles)
            {
                switch (OnDeathAttackType)
                {
                    case 1:
                        newBullet = new Bullet();
                        JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newBullet);
                        newBullet.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                        projectiles.Add(newBullet);
                        break;
                    case 2:
                        newRocket = new Rocket();
                        JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newRocket);
                        newRocket.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                        projectiles.Add(newRocket);
                        break;
                    case 3:
                        newMelee = new Melee();
                        JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newMelee);
                        newRocket.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                        projectiles.Add(newMelee);
                        break;
                    default:
                        break;
                }
            }

            return projectiles;


        }
    }
}
