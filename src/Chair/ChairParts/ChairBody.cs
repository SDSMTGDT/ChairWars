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
using ChairWars.Visual;
using ChairWars.Json;

namespace ChairWars.Mobiles
{
    public class ChairBody : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        public Sprite sprite;
        public string spriteFile { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Polygon collisionBox { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<WeaponSlot> weaponSlots { get; private set; }

        public List<string> weaponSlotFiles;
        public List<string> defaultWeaponFiles;

        public int hitPoints;
        public int defense;
        public float rotation;
        public int mass;
        public string deathExplosionFile;
        

        //public ParticleRequestForm deathExplosion { get; private set; }


        public ChairBody() 
        {
            
            collisionBox = new Polygon();
            weaponSlotFiles = new List<string>();
            weaponSlots = new List<WeaponSlot>();
            defaultWeaponFiles = new List<string>();
        }

        public void Update(Vector2 newCoordinates, float newRotation)
        {
            rotation = newRotation;
            sprite.Update(newCoordinates, rotation);
            foreach( WeaponSlot ws in weaponSlots)
            {
                if (ws != null)
                {
                    ws.Update(newCoordinates, newRotation);
                }
            }
        }

        public void Draw()
        {
            sprite.Draw();
            foreach( WeaponSlot ws in weaponSlots)
            {
                if (ws != null)
                {
                    ws.Draw();
                }
            }
        }

        public void Fire(int fireType)
        {
            foreach( WeaponSlot ws in weaponSlots)
            {
                if(ws.FireType == fireType)
                {
                    ws.FireWeapon();
                }
            }
        }

        public void Reload(int fireType)
        {
            foreach (WeaponSlot ws in weaponSlots)
            {
                if (ws.FireType == fireType)
                {
                    ws.ReloadWeapon();
                }
            }
        }

        public void Initialize()
        {
            int i, iMax;
            WeaponSlot bufferWeaponSlot = null;
            sprite = new Sprite(spriteFile);
            collisionBox = new Polygon(sprite.GetBoundaryPoints());
            foreach (string wepslotstring in weaponSlotFiles)
            {
                JsonExtensions.FromJsonFile(wepslotstring, ref bufferWeaponSlot);
                weaponSlots.Add(bufferWeaponSlot);
            }

            iMax = weaponSlots.Count;
            for(i = 0; i < iMax; i++)
            {
                if (i < defaultWeaponFiles.Count)
                {
                    weaponSlots[i].ChangeWeapon(defaultWeaponFiles[i]);
                }
            }

            

        }
    }
}
