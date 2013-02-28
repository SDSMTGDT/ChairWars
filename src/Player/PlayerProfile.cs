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
using ChairWars.GameStates;
using ChairWars.Json;

namespace ChairWars.Players
{
    public class PlayerProfile : IInitialize        
    {
        private string name;
        //all these data members get added alter wn
        //the classes get built
#if KEYBOARDMOUSE
        public RxInputManager.ControlScheme.ControlScheme<Keys,InGameActions> controlScheme;
#elif XBOX || CONTROLLER
        public RxInputManager.ControlScheme.ControlScheme<Buttons,InGameActions> controlScheme;
#endif
        //Intro intro {get; private set; }
        private int difficulty;
        public int matchesUnlocked;
      //  Texture2D icon;
        public string defaultPlayerChairFile;
        public string defaultPlayerImageFile;
        public string healthBarFile;
        public int kills { get; private set; }
        public int bulletsFired { get; private set; }
        public int bulletHits { get; private set; }
        public int missilesFired { get; private set; }
        public int missileHits { get; private set; }
        public int damageDone { get; private set; }
        public int damageTaken { get; private set; }
        public int timeElapsed { get; private set; }
        public int totalHits { get; private set; }

        // public Achievement[] achievements;


        public PlayerProfile()
        {
            //parse xml file to get all the info for player profile
        }     

        //might need an update function for when matchStats 
        //get implemented

        public void Initialize()
        {

        }
    }
}
