using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Reactive.Linq;
using ChairWars.Arena;
using ChairWars.Visual;
using ChairWars.Mobiles;
using ChairWars.Players;
using ChairWars.Json;

using RxInputManager.Extensions;
using System.Diagnostics;
using System.Threading;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework.Input;

namespace ChairWars.GameStates
{
    
    public enum InGameActions : int
    {
        MoveUp = 0, // will be used for controler version
        FireGroup1 = 1,
        FireGroup2 = 2,
        WeaponCycleLeft = 3,
        WeaponCycleRight = 4,
        Pause = 5,
        Selection = 6,
        Delection = 7,
        MoveDown = 8,
        MoveLeft = 9,
        MoveRight = 10,
        AimUp = 11,
        AimDown = 12,
        AimLeft = 13,
        AimRight = 14,
        Reload1 = 15,
        FireGroup3 = 16,
        FireGroup4 = 17,
        Reload2 = 18,
    }


    public class IncomingState
    {
        public readonly PlayerIndex ID;
        public readonly PlayerProfile Profile;

        public IncomingState(PlayerIndex id, PlayerProfile profile)
        {
            ID = id;
            Profile = profile;
        }
    }

    public class InGameState : AbstractDrawableGameState
    {
        public InGameState(/*  IMatchSetUpState */ params IncomingState[] profiles) : base()
        {
            // IMatchSetupState.Go() or something
            //map is here until a match state is created

            JsonExtensions.FromJsonFileAndInit("./jsonfiles/Levels/Maps/testmap.json", ref Globals.currentMap);
            
            foreach (var profile in profiles)
            {
                Globals.mobileManager.AddPlayer(profile.Profile, profile.ID);
            }
            
            JsonExtensions.FromJsonFileAndInit("./jsonfiles/Levels/Battles/testbattlesequence.json", ref Globals.currentBattleSequence);
            
            Globals.camera.ChangeTarget(Globals.mobileManager.playerList[0].ChairUsed);

            Globals.UpdateSong("Dance");

            

            OnEntered();
        }

        protected override void LoadControls()
        {
#if KEYBOARDMOUSE
            LoadKeyboard(Globals.mobileManager.playerList.First());

#elif CONTROLLER || XBOX 
            foreach (var player in Globals.mobileManager.playerList)
            {
                LoadController(player);       
            }
#endif
        }

#if CONTROLLER || XBOX
        protected void LoadController(Player player)
        {
            var Controller = Globals.InputManager.GetGamePad(player.PlayerID);

            var subs = Controller   //ButtonReleased(player.playerProfile.controlScheme[InGameActions.AimUp].First())
                                 .Sticks(player.playerProfile.controlScheme[InGameActions.AimUp].First(), 150)
                                 .ToSticks()
                                 .Subscribe(sticks => player.Aim((float)Math.Atan2(-sticks.Right.Y, sticks.Right.X)));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.Sticks(player.playerProfile.controlScheme[InGameActions.MoveUp].First(), 150)
                             .ToSticks()
                             .Subscribe(sticks => player.Move((float)Math.Atan2(-sticks.Left.Y, sticks.Left.X)));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsHeld(player.playerProfile.controlScheme[InGameActions.FireGroup1], player.ChairUsed.ChairBodyAccessor.weaponSlots[0].CurrentWeapon.FireRate)
                             .Subscribe(_ => player.Fire(1));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsHeld(player.playerProfile.controlScheme[InGameActions.FireGroup2], player.ChairUsed.ChairBodyAccessor.weaponSlots[0].CurrentWeapon.FireRate)
                 .Subscribe(_ => player.Fire(2));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsPressed(player.playerProfile.controlScheme[InGameActions.WeaponCycleLeft])
                 .Subscribe(_ => Console.WriteLine("Swapped weapons left!"));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsPressed(player.playerProfile.controlScheme[InGameActions.WeaponCycleRight])
                             .Subscribe(_ => Console.WriteLine("Swapped weapons right!"));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsPressed(player.playerProfile.controlScheme[InGameActions.Reload1])
                 .Subscribe(_ => player.Reload(1));

            controlSubscriptions.AddFirst(subs);

            subs = Controller.ButtonsHeld(player.playerProfile.controlScheme[InGameActions.WeaponCycleRight])
                 .Subscribe(_ => player.Reload(2));

            controlSubscriptions.AddFirst(subs);

            LoadPauseControls();
     
        }

        private void LoadPauseControls()
        {
            PauseEnabled = Globals.InputManager.GetGamePad(PlayerIndex.One).ButtonPressedOnce(Buttons.Start)
                           .Take(1)
                           .Subscribe(_ => { Globals.StateManager.Push(Globals.PauseState, Nuclex.Game.States.GameStateModality.Popup); });
        }
#endif

#if KEYBOARDMOUSE
        //IConnectableObservable<KeyboardState> Keyboard;
       // IDisposable KeyboardPause;
        protected void LoadKeyboard(Player player)
        {
            var Keyboard = Globals.InputManager.GetKeyboard();

            var subs = Keyboard.KeysPressed(player.playerProfile.controlScheme[InGameActions.FireGroup1])
                               .Subscribe(_ => player.Fire(1));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.playerProfile.controlScheme[InGameActions.FireGroup2])
                               .Subscribe(_ => player.Fire(2));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.MoveUp])
                           .Subscribe(_ => player.Move(-3.14159f / 2.0f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.MoveDown])
                           .Subscribe(_ => player.Move(3.14159f / 2.0f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.MoveLeft])
                           .Subscribe(_ => player.Move(3.14159f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.MoveRight])
                           .Subscribe(_ => player.Move(0.0f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.AimUp])
                           .Subscribe(_ => player.Aim(4.7124f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.AimDown])
                           .Subscribe(_ => player.Aim(3.14159f / 2.0f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.AimLeft])
           .Subscribe(_ => player.Aim(3.14159f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.AimRight])
                           .Subscribe(_ => player.Aim(0.0f));

            controlSubscriptions.AddFirst(subs);

            subs = Keyboard.KeysPressed(player.ControlScheme[InGameActions.Reload1])
                           .Subscribe(_ => player.Reload(1));

            controlSubscriptions.AddFirst(subs);

            //KeyboardPause = Keyboard.Connect();
            LoadPauseControls();
        }

        
        private void LoadPauseControls()
        {
            PauseEnabled = Globals.InputManager.GetKeyboard().KeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Escape)
                           .Take(1)
                           .Subscribe(_ => { Globals.StateManager.Push(Globals.PauseState, Nuclex.Game.States.GameStateModality.Popup); });
        }

#endif
        private IDisposable PauseEnabled;
        private void UnloadPauseControls()
        {
            PauseEnabled.Dispose();
        }

        protected override void OnPause()
        {
            //KeyboardPause.Dispose();
            UnloadPauseControls();
            base.OnPause();
        }

        protected override void OnResume()
        {
            //if(Keyboard != null)
            //    KeyboardPause = Keyboard.Connect();
            
            LoadPauseControls();
            base.OnResume();
        }

        protected override void OnLeaving()
        {
            PauseEnabled.Dispose();
            base.OnLeaving();
        }


        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            Globals.graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.spriteBatch.Begin();

            Globals.UpdateSpriteBatch(Globals.spriteBatch);
            Globals.currentMap.Draw();
            // TODO: Add your drawing code here
            Globals.mobileManager.Draw();
            Globals.particleEngine.Draw();
            Globals.hudManager.Draw();

            Globals.spriteBatch.End();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!Paused)
            {
                Globals.mobileManager.Update();
                Globals.aiManager.Update();
                Globals.collisionManager.Update();
                Globals.camera.Update();
                Globals.particleEngine.Update();
                Globals.hudManager.Update();
            }
            
            
        }
    }
}
