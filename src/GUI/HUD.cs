using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Visual;
using ChairWars.Json;
using ChairWars.DataStructures;
using ChairWars.Players;
using ChairWars.Enemies;
using ChairWars.Weapons;
using Microsoft.Xna.Framework;

namespace ChairWars.GUI
{
    class HUD
    {
        public Player moniteredPlayer;
        public Sprite playerLifeBar;
        public Sprite reloadBar;
        public List<int> reloadTimers;
        public Sprite [] enemyLifeBars;
        public Enemy [] moniteredEnemies;
        public List<Sprite> weaponBulletSprite;
        public List<Weapon> moniteredWeapons;

        public HUD()
        {
            int i, iMax;
            enemyLifeBars = new Sprite[20];
            weaponBulletSprite = new List<Sprite>();
            reloadTimers = new List<int>();
            moniteredEnemies = new Enemy[20];

            iMax = 20;
            for(i = 0; i < iMax; i++)
            {
                moniteredEnemies[i] = null;
                enemyLifeBars[i] = null;
            }
        }

        public void AddPlayer(Player player)
        {
            moniteredPlayer = player;
            int i, iMax, temp;
            playerLifeBar = new Sprite(moniteredPlayer.healthBarFile);
            playerLifeBar.coordinates = new Vector2();
            iMax = moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots.Count;
            temp = 0;

            reloadBar = new Sprite("HUDIcons/ReloadBar/reloadbar_alphaed");

            for (i = 0; i < iMax; i++)
            {
                reloadTimers.Add(0);
                if (moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon != null)
                {
                    temp++;
                }
            }
            iMax = temp;
            for(i = 0; i < iMax; i++)
            {
                weaponBulletSprite.Add(new Sprite(moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon.hudSpriteFile));
                weaponBulletSprite[i].coordinates = new Vector2(5.0f, weaponBulletSprite[i].image.Height * i + 25.0f);
            }

        }

        public void AddEnemy(Enemy enemy)
        {
            int i, iMax;
            iMax = 20;
            for (i = 0; i < iMax; i++)
            {
                if (moniteredEnemies[i] == null)
                {
                    enemyLifeBars[i] = new Sprite(enemy.healthBarFile);
                    moniteredEnemies[i] = enemy;
                    enemyLifeBars[i].coordinates = new Vector2(Globals.SCREEN_WIDTH / 2, 20.0f * i);
                    break;
                }
            }
        }

        public void RemoveEnemy(Enemy enemy)
        {
            int i, iMax;
            iMax = 20;
            for (i = 0; i < iMax; i++)
            {
                if (moniteredEnemies[i] == enemy)
                {
                    enemyLifeBars[i] = null;
                    moniteredEnemies[i] = null;
                    break;
                }
            }
        }

        public void SetReloadTimer(Weapon currentWeapon, int time)
        {
            int i, iMax;
            iMax = moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots.Count;
            for (i = 0; i < iMax; i++)
            {
                if (moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon == currentWeapon)
                {
                    reloadTimers[i] = time;
                }
            }
        }

        public void Update()
        {
            int i, iMax;
            iMax = reloadTimers.Count;
            for (i = 0; i < iMax; i++)
            {
                if (reloadTimers[i] > 0)
                {
                    reloadTimers[i]--;
                }
            }
        }

        public void Draw()
        {
            int i, j;
            int iMax, jMax;
            Rectangle tempRect;
            Vector2 tempVect = new Vector2();

            //draw life bar
            tempRect = new Rectangle(0, 0,
                (int)(playerLifeBar.image.Width * ((float)moniteredPlayer.ChairUsed.currHP / (float)moniteredPlayer.ChairUsed.maxHP)),
                playerLifeBar.image.Height);

            playerLifeBar.Draw(new Vector2(), tempRect);

            //draw enemy life bars
            iMax = 20;
            for (i = 0; i < iMax; i++)
            {
                if (enemyLifeBars[i] != null)
                {
                    tempRect = new Rectangle(0, 0,
                    (int)(enemyLifeBars[i].image.Width * ((float)moniteredEnemies[i].chairUsed.currHP / (float)moniteredEnemies[i].chairUsed.maxHP)),
                    enemyLifeBars[i].image.Height);
                    enemyLifeBars[i].Draw(new Vector2(), tempRect);
                }
            }

            //draw ammo
            iMax = moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots.Count;
            for (i = 0; i < iMax; i++)
            {
                if (moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon != null)
                {
                    jMax = moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon.CurrentAmmo;
                    tempRect = new Rectangle(0, 0, weaponBulletSprite[i].image.Width, weaponBulletSprite[i].image.Height);
                    for (j = 0; j < jMax; j++)
                    {
                        tempVect.X = weaponBulletSprite[i].coordinates.X + weaponBulletSprite[i].image.Width * j;
                        tempVect.Y = 0.0f;
                        weaponBulletSprite[i].Draw(tempVect, tempRect);
                    }
                }
            }

            //draw reload bars
            iMax = reloadTimers.Count;
            for(i = 0; i < iMax; i++)
            {
                if(reloadTimers[i] > 0)
                {
                    tempRect = new Rectangle(0, 0,
                    (int)(reloadBar.image.Width * ((float)reloadTimers[i] / (float)moniteredPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[i].CurrentWeapon.ReloadRate)),
                    reloadBar.image.Height);
                    tempVect.X = weaponBulletSprite[i].coordinates.X;
                    tempVect.Y = weaponBulletSprite[i].coordinates.Y;
                    reloadBar.Draw(tempVect, tempRect);
                }
            }

        }
    }
}
