using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Json;
using ChairWars.Mobiles;
using Microsoft.Xna.Framework;

namespace ChairWars.Enemies
{
    class Boss : Enemy, IInitialize
    {
        public bool immobile;
        public List<float> healthTriggers;
        public List<bool> healthTriggered;
        public List<bool> singleAttackTriggers;
        public List<string> damfFiles;
        public List<bool> damfTriggers;
        public List<int> continuousSpecialPowers;
        //public List<SpecialWeapon> specialAttacks;

        public Boss()
        {
            healthTriggers = new List<float>();
            healthTriggered = new List<bool>();
            singleAttackTriggers = new List<bool>();
            damfFiles = new List<string>();
            damfTriggers = new List<bool>();
            continuousSpecialPowers = new List<int>();
            //specialAttacks = new List<SpecialWeapon>();
        }

        new public void Initialize()
        {
            base.Initialize();
            base.chairUsed.immobile = immobile;
        }

        public override void Update()
        {
            int i, iMax;
            iMax = healthTriggers.Count;
            for(i = 0; i < iMax; i++)
            {
                if (healthTriggered[i] == false && chairUsed.currHP < chairUsed.maxHP * healthTriggers[i])
                {
                    System.Console.WriteLine("Health triggered");
                    healthTriggered[i] = true;
                    if (damfTriggers[i])
                    {
                        SummonDAMFs();
                    }
                    if (singleAttackTriggers[i])
                    {
                        //use special
                    }
                    if(continuousSpecialPowers[i] != 0)
                    {
                        fireTypes.Add(continuousSpecialPowers[i]);
                    }
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
        
        public void SummonDAMFs()
        {
            int i, iMax;
            iMax = damfFiles.Count;
            for (i = 0; i < iMax; i++)
            {
                Globals.mobileManager.AddEnemy(damfFiles[i], new Vector2(chairUsed.coordinates.X, chairUsed.coordinates.Y));
                Globals.currentBattleSequence.maxEnemyCount++;
            }
        }

        public void UseSpecialWeapon(int weaponIndex)
        {
            //specialweapon fire
        }
    }
}
