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
using ChairWars.Arena;
using ChairWars.Weapons;
using ChairWars.Players;
using ChairWars.Enemies;
using ChairWars.DataStructures;
using ChairWars.Mobiles;

namespace ChairWars.Collisions
{
    class CollisionManager
    {
        public LazyList<Player> playerList;
        public LazyList<Enemy> enemyList;
        public LazyList<Obstacle> obstacleList;

        public CollisionManager()
        {
            playerList = new LazyList<Player>();
            enemyList = new LazyList<Enemy>();
            obstacleList = new LazyList<Obstacle>();
        }

        public float DistanceFormula(float x1, float x2, float y1, float y2)
        {
            return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public void AddPlayer(Player player)
        {
            playerList.Add(player);
        }

        public void AddEnemy(Enemy enemy)
        {
            enemyList.Add(enemy);
        }

        public void AddObstacle(Obstacle obstacle)
        {
            obstacleList.Add(obstacle);
        }

        public void RemovePlayer(Player player)
        {
            playerList.Remove(player);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }

        public void RemoveObstacle(Obstacle obstacle)
        {
            obstacleList.Remove(obstacle);
        }

        public void RemoveProjectile(WeaponSlot weaponslot, IProjectile projectile)
        {
            weaponslot.CurrentWeapon.AttackList.Remove(projectile);
        }

        public void Update()
        {
            List<IProjectile> projectiles;
            Vector2 temp = new Vector2();
            float kineticEnergy = 0.0f;
            int i, j, k, l;
            int iMax, jMax, kMax, lMax;
            Player tempPlayer;
            Enemy tempEnemy;
            IProjectile tempAttack;
            WeaponSlot tempWeaponSlot;
            Obstacle tempObstacle;

            iMax = playerList.Alive;
            jMax = enemyList.Alive;
            for(i = 0; i < iMax; i++)
            {
                tempPlayer = playerList[i];
                for(j = 0; j < jMax; j++)
                {
                    tempEnemy = enemyList[j];
                    if (DistanceFormula(tempEnemy.chairUsed.coordinates.X, tempPlayer.ChairUsed.coordinates.X,
                    tempEnemy.chairUsed.coordinates.Y, tempPlayer.ChairUsed.coordinates.Y)
                    < tempPlayer.ChairUsed.collisionBox.roughRadius * 2)
                    {
                        if (tempPlayer.ChairUsed.collisionBox.CollisionTest(tempEnemy.chairUsed.collisionBox, out temp))
                        {
                            kineticEnergy = tempPlayer.ChairUsed.totalMass * tempPlayer.ChairUsed.forceAccumulator.Length();
                            tempEnemy.chairUsed.HandleCollision(-temp, kineticEnergy);
                            kineticEnergy = tempEnemy.ChairUsed.totalMass * tempEnemy.ChairUsed.forceAccumulator.Length();
                            tempPlayer.ChairUsed.HandleCollision(temp, kineticEnergy);
                        }
                    }
                }
            }

            //enemy colliding with enemies.
            iMax = jMax;
            for (i = 0; i < iMax; i++)
            {
                for (j = 0; j < jMax; j++)
                {
                    if (i != j)
                    {
                        if (DistanceFormula(enemyList[i].chairUsed.coordinates.X, enemyList[j].ChairUsed.coordinates.X,
                        enemyList[i].chairUsed.coordinates.Y, enemyList[j].ChairUsed.coordinates.Y)
                        < enemyList[i].ChairUsed.collisionBox.roughRadius * 2)
                        {
                            if (enemyList[i].chairUsed.collisionBox.CollisionTest(enemyList[j].chairUsed.collisionBox, out temp))
                            {
                                kineticEnergy = enemyList[i].ChairUsed.totalMass * enemyList[i].ChairUsed.forceAccumulator.Length();
                                enemyList[j].chairUsed.HandleCollision(-temp, kineticEnergy);
                                kineticEnergy = enemyList[j].ChairUsed.totalMass * enemyList[j].ChairUsed.forceAccumulator.Length();
                                enemyList[i].ChairUsed.HandleCollision(temp, kineticEnergy);
                            }
                        }
                    }
                }
            }

            iMax = playerList.Alive;
            for (i = 0; i < iMax; i++)
            {
                tempPlayer = playerList[i];
                jMax = tempPlayer.ChairUsed.ChairBodyAccessor.weaponSlots.Count;
                for (j = 0; j < jMax; j++)
                {
                    tempWeaponSlot = tempPlayer.ChairUsed.ChairBodyAccessor.weaponSlots[j];
                    if (tempWeaponSlot.CurrentWeapon == null)
                    {
                        continue;
                    }
                    kMax = tempWeaponSlot.CurrentWeapon.AttackList.Alive;
                    for (k = 0; k < kMax; k++)
                    {
                        tempAttack = tempWeaponSlot.CurrentWeapon.AttackList[k];
                        lMax = enemyList.Alive;
                        for (l = 0; l < lMax; l++)
                        {
                            if (tempAttack.Destroyed)
                            {
                                RemoveProjectile(tempWeaponSlot, tempAttack);
                            }
                            else
                            {
                                tempEnemy = enemyList[l];
                                if (DistanceFormula(tempAttack.CollisionBox.GetCenter().X, tempEnemy.ChairUsed.coordinates.X,
                                tempAttack.CollisionBox.GetCenter().Y, tempEnemy.ChairUsed.coordinates.Y)
                                < tempEnemy.ChairUsed.collisionBox.roughRadius * 2)
                                {
                                    if (tempAttack.InvincibleFrames == 0 && tempAttack.CollisionBox.CollisionTest(tempEnemy.ChairUsed.collisionBox, out temp))
                                    {
                                        tempEnemy.ChairUsed.GetDamaged(tempAttack);
                                        tempEnemy.ChairUsed.HandleCollision(-temp, tempAttack.InitialVelocity * tempAttack.ProjectileMass);
                                        projectiles = tempAttack.OnDeath();
                                        foreach (IProjectile projectile in projectiles)
                                        {
                                            tempWeaponSlot.CurrentWeapon.AttackList.Add(projectile);
                                        }
                                        projectiles.Clear();
                                        RemoveProjectile(tempWeaponSlot, tempAttack);
                                    }
                                }
                            }
                        }
                        lMax = obstacleList.Alive;
                        for (l = 0; l < lMax; l++)
                        {
                            if (tempAttack.InvincibleFrames == 0 && tempAttack.CollisionBox.CollisionTest(obstacleList[l].collisionBox))
                            {
                                projectiles = tempAttack.OnDeath();
                                foreach (IProjectile projectile in projectiles)
                                {
                                    tempWeaponSlot.CurrentWeapon.AttackList.Add(projectile);
                                }
                                projectiles.Clear();
                                RemoveProjectile(tempWeaponSlot, tempAttack);
                            }
                        }
                    }
                }
            }

            //check for enemy projectile hitting player
            iMax = enemyList.Alive;
            for (i = 0; i < iMax; i++)
            {
                tempEnemy = enemyList[i];
                jMax = tempEnemy.ChairUsed.ChairBodyAccessor.weaponSlots.Count;
                for (j = 0; j < jMax; j++)
                {
                    tempWeaponSlot = tempEnemy.ChairUsed.ChairBodyAccessor.weaponSlots[j];
                    if (tempWeaponSlot.CurrentWeapon == null)
                    {
                        continue;
                    }
                    kMax = tempWeaponSlot.CurrentWeapon.AttackList.Alive;
                    for (k = 0; k < kMax; k++)
                    {
                        tempAttack = tempWeaponSlot.CurrentWeapon.AttackList[k];
                        lMax = playerList.Alive;
                        for (l = 0; l < lMax; l++)
                        {
                            if (tempAttack.Destroyed)
                            {
                                RemoveProjectile(tempWeaponSlot, tempAttack);
                            }
                            else
                            {
                                tempPlayer = playerList[l];
                                if (DistanceFormula(tempAttack.CollisionBox.GetCenter().X, tempPlayer.ChairUsed.coordinates.X,
                                tempAttack.CollisionBox.GetCenter().Y, tempPlayer.ChairUsed.coordinates.Y)
                                < tempPlayer.ChairUsed.collisionBox.roughRadius * 2)
                                {
                                    if (tempAttack.InvincibleFrames == 0 && tempAttack.CollisionBox.CollisionTest(tempPlayer.ChairUsed.collisionBox, out temp))
                                    {
                                        tempPlayer.ChairUsed.GetDamaged(tempAttack);
                                        tempPlayer.ChairUsed.HandleCollision(-temp, tempAttack.InitialVelocity * tempAttack.ProjectileMass);
                                        projectiles = tempAttack.OnDeath();
                                        foreach (IProjectile projectile in projectiles)
                                        {
                                            tempWeaponSlot.CurrentWeapon.AttackList.Add(projectile);
                                        }
                                        projectiles.Clear();
                                        RemoveProjectile(tempWeaponSlot, tempAttack);
                                    }
                                }
                            }
                        }
                        lMax = obstacleList.Alive;
                        for (l = 0; l < lMax; l++)
                        {
                            if (tempAttack.InvincibleFrames == 0 && tempAttack.CollisionBox.CollisionTest(obstacleList[l].collisionBox))
                            {
                                projectiles = tempAttack.OnDeath();
                                foreach (IProjectile projectile in projectiles)
                                {
                                    tempWeaponSlot.CurrentWeapon.AttackList.Add(projectile);
                                }
                                projectiles.Clear();
                                RemoveProjectile(tempWeaponSlot, tempAttack);
                            }
                        }
                    }
                }
            }

            iMax = obstacleList.Alive;
            for (i = 0; i < iMax; i++)
            {
                tempObstacle = obstacleList[i];
                jMax = enemyList.Alive;
                for (j = 0; j < jMax; j++)
                {
                    tempEnemy = enemyList[j];
                    if (tempObstacle.collisionBox.CollisionTest(tempEnemy.ChairUsed.collisionBox, out temp))
                    {
                        tempEnemy.ChairUsed.HandleCollision(-temp, tempEnemy.chairUsed.totalMass * tempEnemy.chairUsed.forceAccumulator.Length());
                    }
                }
                jMax = playerList.Alive;
                for (j = 0; j < jMax; j++)
                {
                    tempPlayer = playerList[j];
                    if (tempObstacle.collisionBox.CollisionTest(tempPlayer.ChairUsed.collisionBox, out temp))
                    {
                        tempPlayer.ChairUsed.HandleCollision(-temp, tempPlayer.ChairUsed.totalMass * tempPlayer.ChairUsed.forceAccumulator.Length());
                    }
                }
            }
        }
    }
}
