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
using Nuclex.UserInterface;
using Nuclex.Input.Devices;
using Nuclex.Input;
using Nuclex.Game.States;
using RxInputManager.Gui;
using System.Reactive.Linq;


namespace ChairWars.GameStates
{


    public class MainMenuState : AbstractDrawableGameState
    {
        private Viewport viewport;
        private RxGuiManager gui;
        private guiTest test;
        private Screen mainScreen;

        public MainMenuState(RxGuiManager guiManager)
        {
            this.gui = guiManager;
            
        }


        protected override void OnEntered()
        {
            mainScreen = new Screen(viewport.Width, viewport.Height);
            this.gui.Screen = mainScreen;


            mainScreen.Desktop.Bounds = new UniRectangle(
              new UniScalar(0.0f), new UniScalar(0.0f), // x and y
              new UniScalar(0.0f), new UniScalar(0.0f) // width and height
            );

            test = new guiTest();
            mainScreen.Desktop.Children.Add(test);
            
            Nuclex.UserInterface.Controls.Desktop.ButtonControl newGameButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            newGameButton.Text = "New Game";
            newGameButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -90.0f), new UniScalar(1.0f, -120.0f), 100, 32
            );
            //  newGameButton.Pressed += new EventHandler(newGameButton_Pressed);
            mainScreen.Desktop.Children.Add(newGameButton);

            base.OnEntered();
            
        }

        public event EventHandler OkButtonEvent
        {
            add { test.okButton.Pressed += value; }
            remove { test.okButton.Pressed -= value; }
        }

        protected override void OnLeaving()
        {
            gui.Screen = null;

            base.OnLeaving();
        }

        protected override void OnResume()
        {
            gui.Screen = mainScreen;
            base.UnloadControls();
            base.OnResume();
        }

        // fix me
        protected override void OnPause()
        {
            gui.Screen = null;
            base.UnloadControls();
            base.OnPause();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.gui.Draw(gameTime);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.gui.Update(gameTime);
        }

        protected override void LoadControls()
        {
#if XBOX || CONTROLLER
            var controller = Globals.InputManager.GetGamePad(PlayerIndex.One);

            var sub = controller.ButtonPressedOnce(Buttons.Start)
                                 .Take(1)
                                 .Subscribe(_ => Globals.StateManager.Pop());

            controlSubscriptions.AddFirst(sub);

#else
            var keyboard = Globals.InputManager.GetKeyboard();
            var mouse = Globals.InputManager.GetMouse();

            var sub = keyboard.KeyPressedOnce(Keys.Escape)
                              .Take(1)
                              .Subscribe(_ => Globals.StateManager.Pop());

            controlSubscriptions.AddFirst(sub);

            sub = Observable.FromEventPattern<EventArgs>(test.okButton, "Pressed")
                            .Take(1)
                            .Subscribe(_ => Globals.StateManager.Pop());

            controlSubscriptions.AddFirst(sub);
#endif
        }

    }
}
