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
using ChairWars.Enemies;

namespace ChairWars.AI
{
    class AIManager
    {
        float tempAngle;
        float tempDistance;
        Vector2 tempVector;
        Vector2 enemyCords;
        Vector2 playerCords;
        Enemy currEnemy;

        public AIManager()
        {
            tempVector = new Vector2(0.0f, 0.0f);
            enemyCords = new Vector2();
            playerCords = new Vector2();
        }

        public void BasicRangedAI()
        {
            CalculatePlayerInfluence();
            BasicAim(tempAngle);
            BasicFire(tempAngle, tempDistance);
            CalculateOtherEnemyInfluence();
            BasicMove(tempVector);
        }

        public void BasicRangedBossAI()
        {
            CalculatePlayerInfluence();
            BasicAim(tempAngle);
            AdvancedFire(tempAngle, tempDistance);
            BasicMove(tempVector);
        }

        public void CalculatePlayerInfluence()
        {
            playerCords.X = Globals.mobileManager.playerList[0].ChairUsed.coordinates.X;
            playerCords.Y = Globals.mobileManager.playerList[0].ChairUsed.coordinates.Y;
            enemyCords.X = currEnemy.chairUsed.coordinates.X;
            enemyCords.Y = currEnemy.chairUsed.coordinates.Y;
            //calc angle to player
            
            tempAngle = (float)Math.Atan2(playerCords.Y - enemyCords.Y, playerCords.X - enemyCords.X);
            tempDistance = DistanceFormula(playerCords.X, enemyCords.X, playerCords.Y, enemyCords.Y);
            tempVector.Y += (float)(1.2 * Math.Sin(tempAngle)) / (tempDistance - currEnemy.desiredDistance);
            tempVector.X += (float)(1.2 * Math.Cos(tempAngle)) / (tempDistance - currEnemy.desiredDistance);
            
        }

        public void CalculateOtherEnemyInfluence()
        {
            enemyCords.X = currEnemy.chairUsed.coordinates.X;
            enemyCords.Y = currEnemy.chairUsed.coordinates.Y;

            foreach (Enemy otherE in Globals.mobileManager.enemyList)
            {
                if (currEnemy == otherE)
                {
                    continue;
                }
                tempAngle = (float)Math.Atan2(enemyCords.Y - otherE.chairUsed.coordinates.Y, enemyCords.X - otherE.chairUsed.coordinates.X);
                tempDistance = DistanceFormula(enemyCords.X, otherE.chairUsed.coordinates.X, enemyCords.Y, otherE.chairUsed.coordinates.Y);
                tempVector.Y += (float)((Math.Sin(tempAngle) / (1.0 + tempDistance)) / (float)Globals.mobileManager.enemyList.Alive);
                tempVector.X += (float)((Math.Cos(tempAngle) / (1.0 + tempDistance)) / (float)Globals.mobileManager.enemyList.Alive);
            }
        }

        public void BasicMove(Vector2 moveVector)
        {
            if (moveVector.Length() < 0.1f)
            {
                moveVector.Normalize();
                currEnemy.Move((float)Math.Atan2(tempVector.Y, tempVector.X));
            }
        }

        public void BasicAim(float aimAngle)
        {
            currEnemy.Aim(aimAngle);
        }

        public void BasicFire(float angleToPlayer, float distanceToPlayer)
        {
            angleToPlayer = Math.Abs(angleToPlayer - currEnemy.chairUsed.currentBodyRotation);
            angleToPlayer = angleToPlayer % ((float)Math.PI * 2);
            if (angleToPlayer < 0.1f && distanceToPlayer < currEnemy.desiredDistance * 2)
            {
                currEnemy.chairUsed.Fire(currEnemy.fireTypes);
            }
        }

        public void AdvancedFire(float angleToPlayer, float distanceToPlayer)
        {
            angleToPlayer = Math.Abs(angleToPlayer - currEnemy.chairUsed.currentBodyRotation);
            angleToPlayer = angleToPlayer % ((float)Math.PI * 2);
            if (angleToPlayer < 0.15f)
            {
                currEnemy.chairUsed.Fire(currEnemy.fireTypes);
            }
        }

        public float DistanceFormula(float x1, float x2, float y1, float y2)
        {
            return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public void Update()
        {
            int i;
            int iMax;
            iMax = Globals.mobileManager.enemyList.Alive;
            for (i = 0; i < iMax; i++)
            {
                currEnemy = Globals.mobileManager.enemyList[i];
                tempAngle = 0.0f;
                tempDistance = 0.0f;
                tempVector.X = 0.0f;
                tempVector.Y = 0.0f;

                switch (currEnemy.aiType)
                {
                    case 1:
                        BasicRangedAI();
                        break;
                    case 2:
                        break;
                    case 3:
                        BasicRangedBossAI();
                        break;
                    case 4:
                        break;
                    default:
                        BasicRangedAI();
                        break;
                }
                
            }
        }

    }
}
