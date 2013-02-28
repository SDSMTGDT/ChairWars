using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RxInputManager;
using ChairWars.AI;
using ChairWars.Mobiles;
using ChairWars.Collisions;
using ChairWars.Visual;
using ChairWars.GameStates;
using ChairWars.Players;
using ChairWars.Particles;
using ChairWars.GUI;
using RxInputManager.ControlScheme;
using Nuclex.UserInterface;
using Nuclex.Input.Devices;
using Nuclex.Game.States;
using RxInputManager.Gui;
using System.Reactive.Linq;

namespace ChairWars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MobileManager mobileManager;
        AIManager aiManager;
        CollisionManager collisionManager;
        Random randomNumberGenerator;
        PlayerProfile my_profile;
        AudioEngine audioEngine;
        WaveBank soundEffectWaveBank;
        SoundBank soundEffectSoundBank;
        WaveBank musicWaveBank;
        SoundBank musicSoundBank;
        Camera camera;
        ParticleEngine particleEngine;
        RxGuiManager gui;
        HUD hudManger;
        

        public Game1()
        {
            Globals.StateManager = new GameStateManager(Services);
            graphics = new GraphicsDeviceManager(this);
            //input = new Nuclex.Input.InputManager(Services);
            
            Globals.InputManager = new RxInputService(this.Services);
            gui = new RxGuiManager(graphics, Globals.InputManager);
            //this.Components.Add(Globals.InputManager);
            this.Components.Add(Globals.InputManager);
            this.Components.Add(gui);
            //this.gui = new GuiManager(Services);
            //Components.Add(this.gui);
            //Components.Add(input);
            my_profile = new PlayerProfile();
     
#if KEYBOARDMOUSE
            //my_profile.controlScheme = new ControlScheme<Keys, InGameActions>("Default Keyboard & Mouse", ControlSchemeHelper.GetAllValues<InGameActions>());

            //my_profile.controlScheme.AddKey(InGameActions.MoveDown, Keys.Down);
            //my_profile.controlScheme.AddKey(InGameActions.MoveUp, Keys.Up);
            //my_profile.controlScheme.AddKey(InGameActions.MoveLeft, Keys.Left);
            //my_profile.controlScheme.AddKey(InGameActions.MoveRight, Keys.Right);
            //my_profile.controlScheme.AddKey(InGameActions.FireGroup1, Keys.LeftShift);
            //my_profile.controlScheme.AddKey(InGameActions.AimUp, Keys.W);
            //my_profile.controlScheme.AddKey(InGameActions.AimDown, Keys.S);
            //my_profile.controlScheme.AddKey(InGameActions.AimLeft, Keys.A);
            //my_profile.controlScheme.AddKey(InGameActions.AimRight, Keys.D);
            //my_profile.controlScheme.AddKey(InGameActions.Reload1, Keys.R);

            //Json.JsonExtensions.ToJsonFile("SampleOneProfileKM.json", ref my_profile);
            Json.JsonExtensions.FromJsonFile("jsonfiles/Player/SampleOneProfileKM.json", ref my_profile);
#elif XBOX || CONTROLLER
            //my_profile.controlScheme = new ControlScheme<Buttons, InGameActions>("Default Controller", ControlSchemeHelper.GetAllValues<InGameActions>());

            //my_profile.controlScheme.AddKey(InGameActions.MoveUp, Buttons.LeftStick);
            //my_profile.controlScheme.AddKey(InGameActions.AimUp, Buttons.RightStick);

            //my_profile.controlScheme.AddKey(InGameActions.FireGroup1, Buttons.LeftTrigger);
           
            //my_profile.controlScheme.AddKey(InGameActions.Reload1, Buttons.A);
            //my_profile.controlScheme.AddKey(InGameActions.Reload2, Buttons.B);

            //Json.JsonExtensions.ToJsonFile("SampleOneProfile.json", ref my_profile);
            Json.JsonExtensions.FromJsonFile("jsonfiles/Player/SampleOneProfile.json", ref my_profile);

#endif
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Globals.LoadGlobals();
            Viewport viewport = GraphicsDevice.Viewport;
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);

            IsMouseVisible = true;
            base.Initialize();
        }

        private InGameState currentState;
        private MainMenuState menuState;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mobileManager = new MobileManager();
            aiManager = new AIManager();
            collisionManager = new CollisionManager();
            randomNumberGenerator = new Random();
            camera = new Camera();
            audioEngine = new AudioEngine(".\\Sounds\\Win\\chairwars.xgs");
            soundEffectWaveBank = new WaveBank(audioEngine, ".\\Sounds\\Win\\SoundEffects.xwb");
            soundEffectSoundBank = new SoundBank(audioEngine, ".\\Sounds\\Win\\SoundEffects.xsb");
            musicWaveBank = new WaveBank(audioEngine, ".\\Sounds\\Win\\Music.xwb");
            musicSoundBank = new SoundBank(audioEngine, ".\\Sounds\\Win\\Music.xsb");
            particleEngine = new ParticleEngine();
            hudManger = new HUD();
            Globals.UpdateContentManager(Content);
            Globals.UpdateGraphicsDeviceManager(graphics);
            Globals.UpdateMobileManager(mobileManager);
            Globals.UpdateAIManager(aiManager);
            Globals.UpdateCollisionManager(collisionManager);
            Globals.UpdateRNG(randomNumberGenerator);
            Globals.UpdateSpriteBatch(spriteBatch);
            Globals.UpdateCamera(camera);
            Globals.UpdateAudioEngine(audioEngine);
            Globals.UpdateWaveBankSoundEffects(soundEffectWaveBank);
            Globals.UpdateSoundBankSoundEffects(soundEffectSoundBank);
            Globals.UpdateWaveBankMusic(musicWaveBank);
            Globals.UpdateSoundBankMusic(musicSoundBank);
            Globals.UpdateParticleEngine(particleEngine);
            Globals.UpdateHUD(hudManger);

            Viewport viewport = GraphicsDevice.Viewport;
            menuState = new MainMenuState(gui);

            Globals.PauseState = menuState;

            currentState = new InGameState(new IncomingState(PlayerIndex.One, my_profile));
            Globals.StateManager.Push(currentState, GameStateModality.Exclusive);
            //stateManager.Push(menuState, GameStateModality.Exclusive);

            //menuState.OkButtonEvent += (s, o) => { stateManager.Pop(); stateManager.Push(currentState, GameStateModality.Exclusive); };
            
            //var gameStatePush = Globals.InputManager.GetGamePad(PlayerIndex.One).ButtonPressedOnce(Buttons.A).Take(1).Subscribe(_ => { stateManager.Pop(); stateManager.Push(currentState, GameStateModality.Exclusive); });

            //Globals.InputManager.GetGamePad(PlayerIndex.One).ButtonPressedOnce(Buttons.Start).Subscribe(_ => { PauseMenuSwap(); });

            
            //stateManager.Switch(menuState);
            //menuState = new MainMenuState();

            //currentState.En

            

           // Globals.mobileManager.AddPlayer(null, PlayerIndex.One);
            //Globals.mobileManager.AddEnemy(100.0f/*"not used"*/);
            //Globals.mobileManager.AddEnemy(200.0f/*"not used"*/);
           // Globals.mobileManager.AddEnemy(400.0f/*"not used"*/);
           // Map testMap = new Map();
            // TODO: use this.Content to load your game content here
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Globals.UpdateGameTime(gameTime);
           // Globals.iOEvents.Update(gameTime);
            //Globals.mobileManager.Update();
            //Globals.aiManager.Update();
            //Globals.collisionManager.Update();

            //menuState.Update(gameTime);
            //currentState.Update(gameTime);
            Globals.StateManager.Update(gameTime);

            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin();

            //Globals.UpdateSpriteBatch(spriteBatch);
            //// TODO: Add your drawing code here
            //Globals.mobileManager.Draw();

            //spriteBatch.End();

            //currentState.Draw(gameTime);
            Globals.StateManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
