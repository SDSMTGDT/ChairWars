using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Json;
using ChairWars.Enemies;
using ChairWars.GameStates;
using ChairWars.Players;
using Nuclex.Game.States;
using Microsoft.Xna.Framework;

namespace ChairWars.Arena
{
    class BattleSequence : IInitialize
    {
        [Newtonsoft.Json.JsonIgnore]
        public int currDeathCount;

        [Newtonsoft.Json.JsonIgnore]
        public int maxEnemyCount;

        public List<string> enemySequence;
        public List<bool> isBoss;
        public List<int> spawnConditions;
        public List<Vector2> spawnLocations;

        public BattleSequence()
        {
            enemySequence = new List<string>();
            isBoss = new List<bool>();
            spawnConditions = new List<int>();
            spawnLocations = new List<Vector2>();
            currDeathCount = 0;
        }

        public void Initialize()
        {
            for (int i = 0; i < spawnConditions.Count; i++)
            {
                Vector2 mapBounds = new Vector2();
                mapBounds.X = Globals.currentMap.mapBounds.X * Globals.currentMap.enemyStartLocation.X;
                mapBounds.Y = Globals.currentMap.mapBounds.Y * Globals.currentMap.enemyStartLocation.Y;
                if (spawnConditions[i] == currDeathCount)
                {
                    if (isBoss[i] == false)
                    {
                        Globals.mobileManager.AddEnemy(enemySequence[i], spawnLocations[i] + mapBounds);
                    }
                    if (isBoss[i] == true)
                    {
                        Globals.mobileManager.AddBoss(enemySequence[i], spawnLocations[i] + mapBounds);
                    }
                }
            }
            maxEnemyCount = enemySequence.Count;
        }

        public void OnEnemyDeath()
        {
            InGameState currentState;
            PlayerProfile my_profile;

            currDeathCount++;
            if (currDeathCount == maxEnemyCount)
            {
                Globals.StateManager.Pop();
                Globals.mobileManager.RemovePlayer(Globals.mobileManager.playerList[0]);
                my_profile = new PlayerProfile();
#if KEYBOARDMOUSE
                Json.JsonExtensions.FromJsonFile("jsonfiles/Player/SampleOneProfileKM.json", ref my_profile);
#elif XBOX || CONTROLLER
                Json.JsonExtensions.FromJsonFile("jsonfiles/Player/SampleOneProfile.json", ref my_profile);
#endif
                System.Console.WriteLine("Match over.");
                currentState = new InGameState(new IncomingState(PlayerIndex.One, my_profile));
                Globals.StateManager.Push(currentState, GameStateModality.Exclusive);
                return;
            }
            for (int i = 0; i < spawnConditions.Count; i++)
            {
                if (spawnConditions[i] == currDeathCount)
                {
                    Vector2 mapBounds = new Vector2();
                    mapBounds.X = Globals.currentMap.mapBounds.X * Globals.currentMap.enemyStartLocation.X;
                    mapBounds.Y = Globals.currentMap.mapBounds.Y * Globals.currentMap.enemyStartLocation.Y;
                    if (isBoss[i] == false)
                    {
                        Globals.mobileManager.AddEnemy(enemySequence[i], spawnLocations[i] + mapBounds);
                    }
                    if (isBoss[i] == true)
                    {
                        Globals.mobileManager.AddBoss(enemySequence[i], spawnLocations[i] + mapBounds);
                    }
                }
            }
        }
    }
}
