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
using ChairWars.DataStructures;
using ChairWars.Players;
using ChairWars.Enemies;
using ChairWars.Json;

namespace ChairWars.Mobiles
{
    class MobileManager
    {
        public LazyList<Player> playerList;
        public LazyList<Enemy> enemyList;

        public MobileManager()
        {
            playerList = new LazyList<Player>();
            enemyList = new LazyList<Enemy>();
        }

        public void AddPlayer(PlayerProfile profile, PlayerIndex ID)
        {
            playerList.Add(new Player(profile, ID));
        }

        public void RemovePlayer(Player player)
        {
            Globals.collisionManager.RemovePlayer(player);
            playerList.Remove(player);
        }

        public void AddEnemy(string fileName, Vector2 spawnLocation)
        {
            Enemy tempEnemy = new Enemy();
            JsonExtensions.FromJsonFileAndInit(fileName, ref tempEnemy);
            tempEnemy.SetCoordinates(spawnLocation);
            enemyList.Add(tempEnemy);
            Globals.hudManager.AddEnemy(tempEnemy);
        }

        public void AddBoss(string fileName, Vector2 spawnLocation)
        {
            Boss tempBoss = new Boss();
            JsonExtensions.FromJsonFileAndInit(fileName, ref tempBoss);
            tempBoss.SetCoordinates(spawnLocation);
            enemyList.Add(tempBoss);
            Globals.hudManager.AddEnemy(tempBoss);
        }

        public void RemoveEnemy(Chair deadChair)
        {
            for (int i = 0; i < enemyList.Alive; i++)
            {
                if (enemyList[i].chairUsed == deadChair)
                {
                    Globals.collisionManager.RemoveEnemy(enemyList[i]);
                    Globals.hudManager.RemoveEnemy(enemyList[i]);
                    enemyList.RemoveAt(i);
                    Globals.currentBattleSequence.OnEnemyDeath();
                    
                    break;
                }
            }
        }

        public void ClearEnemies()
        {
            enemyList.Clear();
        }

        public void Update()
        {
            int i, iMax;
            iMax = playerList.Alive;
            for(i = 0; i < iMax; i++)
            {
                playerList[i].Update();
            }

            iMax = enemyList.Alive;
            for(i = 0; i < iMax; i++)
            {
                enemyList[i].Update();
            }
        }

        public void Draw()
        {
            int i, iMax;
            iMax = playerList.Alive;
            for (i = 0; i < iMax; i++)
            {
                playerList[i].Draw();
            }

            iMax = enemyList.Alive;
            for (i = 0; i < iMax; i++)
            {
                enemyList[i].Draw();
            }
        }
    }
}
