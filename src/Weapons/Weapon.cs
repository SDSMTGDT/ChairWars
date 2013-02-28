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
using ChairWars.DataStructures;

namespace ChairWars.Weapons
{
    public class Weapon
    {
        public Vector2 MountingPin { get; set; }
        public int Size { get; set; }
        public double Weight { get; set; }
        public int FireRate { get; set; }
        public int Cooldown { get; set; }
        public int ClipSize { get; set; }
        public int CurrentAmmo { get; set; }
        public int ReloadRate { get; set; }
        public int AttackType { get; set; }
        public int ProjectileSpeed { get; set; }
        public int ProjectileWeight { get; set; }

        public string hudSpriteFile { get; set; }
        public string SpriteFile { get; set; }
        public string soundCueFire { get; set; }
        public string soundCueReload { get; set; }
        public List<AttackFile> AttackFiles { get; set; }

        public Sprite Image { get; private set; }


        public LazyList<IProjectile> AttackList;


        public Weapon()
        {
            AttackList = new LazyList<IProjectile>();
        }

        public void Initialize()
        {
            if (AttackFiles == null)
                AttackFiles = new List<AttackFile>();
            Image = new Sprite(SpriteFile);
            CurrentAmmo = ClipSize;
        }

        public void fireAttack(Vector2 position, float rotation)
        {
            Bullet newBullet = null;
            Rocket newRocket = null;
            Melee  newMelee  = null;

            if (Cooldown == 0 && CurrentAmmo != 0)
            {
                Cooldown = FireRate;
                CurrentAmmo--;
                if (!string.IsNullOrEmpty(soundCueFire))
                {
                    Globals.soundBankSoundEffects.PlayCue(soundCueFire);
                }
                foreach (AttackFile AttackFile in AttackFiles)
                {
                    switch (AttackType)
                    {
                        case 1:
                            newBullet = new Bullet();
                            JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newBullet);
                            newBullet.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                            newBullet.Update();
                            AttackList.Add(newBullet);
                            break;
                        case 2:
                            newRocket = new Rocket();
                            JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newRocket);
                            newRocket.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                            newRocket.Update();
                            AttackList.Add(newRocket);
                            break;
                        case 3:
                            newMelee = new Melee();
                            JsonExtensions.FromJsonFile(AttackFile.AttackFilename, ref newMelee);
                            newRocket.InitializeProjectile(position, rotation, AttackFile.RotationOffset);
                            newMelee.Update();
                            AttackList.Add(newMelee);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Reload()
        {
            Cooldown = ReloadRate;
            CurrentAmmo = ClipSize;
            Globals.hudManager.SetReloadTimer(this, ReloadRate);
            if (!string.IsNullOrEmpty(soundCueReload))
            {
                Globals.soundBankSoundEffects.PlayCue(soundCueReload);
            }
        }

        public void Draw()
        {
            int i, iMax;
            iMax = AttackList.Alive;
            for (i = 0; i < iMax; i++)
            {
                AttackList[i].ProjectileSprite.Draw();
            }

            Image.Draw();
        }


        public void Update(Vector2 position, float rotation)
        {
            int i, iMax;
            List<IProjectile> tempAttackList = new List<IProjectile>();
            if (Cooldown > 0)
                Cooldown--;
            if (CurrentAmmo <= 0)
                Reload();

            iMax = AttackList.Alive;
            for (i = 0; i < iMax; i++ )
            {
                AttackList[i].Update();
                if (AttackList[i].Lifespan == 0)
                {
                    tempAttackList.AddRange(AttackList[i].OnDeath());
                }
            }

            if (AttackList.Count - AttackList.Alive > 50)
            {
                AttackList.ClearDead();
            }

            foreach (IProjectile attack in tempAttackList)
            {
                AttackList.Add(attack);
            }


            //Update Sprite rotation and position
            Image.Update(position, rotation);
        }
    }
}
