using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChairWars.Json;


namespace ChairWars.Weapons
{
    public class WeaponSlot
    {
        public Weapon CurrentWeapon { get; private set; }
        public double WeightLimit { get; set; }
        public int SizeLimit { get; set; }
        public Vector2 MountingHole { get; set; }
        public Vector2 WeaponPosition { get; private set; }
        public float Rotation { get; private set; }
        public int FireType { get; set; }
        public string currentWeaponFile { get; set; }
        public bool RotateWeaponVertical { get; set; }

        public WeaponSlot()
        {
            currentWeaponFile = null;
        }

        public bool ChangeWeapon(string newWeaponFile)
        {
            if (LoadWeapon(newWeaponFile))
            {
                currentWeaponFile = newWeaponFile;
                return true;
            }
            return false;
        }

        public bool LoadWeapon()
        {
            return LoadWeapon(currentWeaponFile);
        }

        public bool LoadWeapon(string newWeaponFile)
        {
            if (newWeaponFile == null || newWeaponFile == "")
            {
                return false;
            }

            Weapon newWeapon = new Weapon();
            JsonExtensions.FromJsonFile(newWeaponFile, ref newWeapon);
            newWeapon.Initialize();
            if (newWeapon.Size < SizeLimit &&
                newWeapon.Weight < WeightLimit)
            {
                CurrentWeapon = newWeapon;
                CurrentWeapon.Image.SetFlip(RotateWeaponVertical);
                return true;
            }
            System.Console.WriteLine("Weapon not loaded.");
            return false;

        }

        public bool ChangeFireType(int fireType = -1)
        {
            if (fireType > 0 && fireType < 5)
            {
                FireType = fireType;
                return true;
            }
            return false;
        }


        public void Draw()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Draw();
            }
        }

        public void FireWeapon()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.fireAttack(WeaponPosition, Rotation);
            }
        }

        public void ReloadWeapon()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Reload();
            }
        }

        public void Update(Vector2 position, float rotation)
        {
            if (CurrentWeapon != null)
            {
                Vector2 rotatedPosition = Rotate(rotation);
                WeaponPosition = new Vector2(position.X - rotatedPosition.X, position.Y - rotatedPosition.Y);
                Rotation = rotation;
                CurrentWeapon.Update(WeaponPosition, rotation);
            }
        }

        private Vector2 Rotate(float rotation)
        {
            float tempX, tempY;
            tempX = (float)(MountingHole.X * Math.Cos(rotation) - MountingHole.Y * Math.Sin(rotation));
            tempY = (float)(MountingHole.X * Math.Sin(rotation) + MountingHole.Y * Math.Cos(rotation));
            return new Vector2(tempX, tempY);
        }


    }
}
