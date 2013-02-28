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
using ChairWars.Mobiles;
using ChairWars.GameStates;
using ChairWars.DataStructures;
using ChairWars.Json;
using ChairWars.Visual;
using RxInputManager.ControlScheme;

namespace ChairWars.Players
{
    public class Player : IsAlive, IInitialize
    {
        private Chair chairUsed;
        public Chair ChairUsed { get { return chairUsed; } private set { chairUsed = value; } }
        //public Intro intro { get; private set; }
        public string playerName { get; private set; }
        private Sprite playerCharacterImage;
        public PlayerProfile playerProfile { get; private set; }
        public int matchesUnlocked { get; set; }
#if KEYBOARDMOUSE
        public RxInputManager.ControlScheme.ControlScheme<Keys, InGameActions> ControlScheme { get { return playerProfile.controlScheme; } }
#elif XBOX || CONTROLLER
        public ControlScheme<Buttons, InGameActions> ControlScheme { get { return playerProfile.controlScheme; } }
#endif
        public PlayerIndex PlayerID { get; private set; }
        private bool hackedInBoost;
        public string healthBarFile;
        public bool Destroyed { get; set; }

        //public List<Announcer> preferedAnnoucers { get; set; } 

        public Player(PlayerProfile profile, PlayerIndex playerID)
        {
            PlayerID = playerID;
            playerProfile = profile;

            this.Initialize();
            /*
             * playerProfile = profile;
             * matchesUnlocked = playerProfile.matchesUnlocked;
             * chairUsed = new Chair("parsedfile.xml");
             * intro = new Intro("parsedfile.xml");
             * control subscriptions
             * Announcer setup
            
            player1 = PlayerIndex.One;
            
            chairUsed.coordinates.Y = 100.0f;
            chairUsed.coordinates.X = 100.0f;
            Globals.collisionManager.AddPlayer(this);
             */
        }

        public void Initialize()
        {
           // PlayerID = PlayerIndex.One;

            healthBarFile = playerProfile.healthBarFile;
            JsonExtensions.FromJsonFileAndInit(playerProfile.defaultPlayerChairFile, ref chairUsed);
            Globals.hudManager.AddPlayer(this);

            playerCharacterImage = new Sprite(playerProfile.defaultPlayerImageFile);
            ChairUsed.SetChairRider(playerCharacterImage);

            chairUsed.coordinates.Y = Globals.currentMap.playerStartLocation.Y * Globals.currentMap.mapBounds.Y;
            chairUsed.coordinates.X = Globals.currentMap.playerStartLocation.X * Globals.currentMap.mapBounds.X;
            Globals.collisionManager.AddPlayer(this);
        }

        private void LoadController()
        {
           // var baseState = Globals.iOEvents.Player(player1);
        }

        public void Update()
        {
            KeyboardState hackedIn = new KeyboardState();
            hackedIn = Keyboard.GetState();
            hackedInBoost = false;
            if (hackedIn.IsKeyDown(Keys.Space))
            {
                hackedInBoost = true;
            }
            if(hackedIn.IsKeyDown(Keys.Q))
            {
                Globals.camera.SetShake(20, 500, true);
            }
            
            //if (hackedIn.IsKeyDown(Keys.Up))
            //{
            //    Move(-3.14159f / 2.0f);
            //}
            //if (hackedIn.IsKeyDown(Keys.Down))
            //{
            //    Move(3.14159f / 2.0f);
            //}
            //if (hackedIn.IsKeyDown(Keys.Left))
            //{
            //    Move(3.14159f);
            //}
            //if (hackedIn.IsKeyDown(Keys.Right))
            //{
            //    Move(0.0f);
            //}
            //if (hackedIn.IsKeyDown(Keys.W))
            //{
            //    Aim(4.7124f);
            //}
            //if (hackedIn.IsKeyDown(Keys.S))
            //{
            //    Aim(3.14159f / 2.0f);
            //}
            //if (hackedIn.IsKeyDown(Keys.A))
            //{
            //    Aim(3.14159f);
            //}
            //if (hackedIn.IsKeyDown(Keys.D))
            //{
            //    Aim(0.0f);
            //}
            //if (hackedIn.IsKeyDown(Keys.R))
            //{
            //    Reload(1);
            //}
            //if (hackedIn.IsKeyDown(Keys.LeftShift))
            //{
            //    Fire();
            //}


            

            chairUsed.Update();
        }

        public void Draw()
        {
            chairUsed.Draw();
        }

        public void IntroBegin()
        {
            //generate inIntro signal
            //perform intro
        }

        public void UpdateStats(/*MatchStats newStats*/)
        {
            //go through and update the playerProfile stats
            //write out new profile XML
        }

        public void Fire(int fireType = 1)
        {
            chairUsed.Fire(fireType);
        }

        public void Reload(int attackType)
        {
            chairUsed.Reload(attackType);
        }

        public void Move(float rotation/*control stuff*/)
        {
            if(hackedInBoost == true)
            {
                Boost(rotation);
            }
            chairUsed.Move(rotation);
        }

        public void Boost(float rotation)
        {
            chairUsed.Boost(rotation);
        }

        public void Aim(float rotation/*control stuff*/)
        {
            float temp = 0.0f;
            temp = rotation;
            chairUsed.Aim(temp);
        }

        public void CheckDeath()
        {
            if (chairUsed.alive == false)
            {
                //do stuff
            }
        }

        public void UpdateChair(Chair newChair)
        {
            chairUsed = newChair;
            //write out to player profile and xml
        }

        
    }
}
