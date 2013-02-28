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
using RxInputManager;
using ChairWars.AI;
using ChairWars.Visual;
using ChairWars.Mobiles;
using ChairWars.Collisions;
using ChairWars.Arena;
using ChairWars.Particles;
using ChairWars.GUI;
using Nuclex.Game.States;

namespace ChairWars
{
    static class Globals
    {
        public static GameTime gameTime;
        public static bool pause;
        public static SpriteBatch spriteBatch;
        public static GraphicsDeviceManager graphicsDeviceManager;
        public static ContentManager contentManager;
        public static MobileManager mobileManager;
        public static AIManager aiManager;
        public static CollisionManager collisionManager;
        public static Random randomNumberGenerator;
        public static RxInputService InputManager;
        public static Camera camera;
        public static Map currentMap;
        public static BattleSequence currentBattleSequence;
        public static AudioEngine audioEngine;
        public static WaveBank waveBankSoundEffects;
        public static SoundBank soundBankSoundEffects;
        public static WaveBank waveBankMusic;
        public static SoundBank soundBankMusic;
        public static ParticleEngine particleEngine;
        public static HUD hudManager;
        public static int SCREEN_WIDTH = 800;
        public static int SCREEN_HEIGHT = 480;

        public static GameStateManager StateManager;
        public static IGameState PauseState;

        public static void LoadGlobals()
        {
            pause = false;
        }

        public static void UpdateGameTime(GameTime gt)
        {
            gameTime = gt;
        }

        public static void UpdateSpriteBatch(SpriteBatch sb)
        {
            spriteBatch = sb;
        }

        public static void UpdateContentManager(ContentManager cm)
        {
            contentManager = cm;
        }

        public static void UpdateGraphicsDeviceManager(GraphicsDeviceManager gdm)
        {
            graphicsDeviceManager = gdm;
        }

        public static void UpdateMobileManager(MobileManager mm)
        {
            mobileManager = mm;
        }

        public static void UpdateAIManager(AIManager aim)
        {
            aiManager = aim;
        }

        public static void UpdateCollisionManager(CollisionManager cm)
        {
            collisionManager = cm;
        }

        public static void UpdateRNG(Random rng)
        {
            randomNumberGenerator = rng;
        }

        public static void UpdateCamera(Camera cam)
        {
            camera = cam;
        }
        public static void UpdateBattleSequence(BattleSequence bs)
        {
            currentBattleSequence = bs;
        }
        public static void UpdateAudioEngine(AudioEngine ae)
        {
            audioEngine = ae;
        }
        public static void UpdateWaveBankSoundEffects(WaveBank wb)
        {
            waveBankSoundEffects = wb;
        }
        public static void UpdateSoundBankSoundEffects(SoundBank sb)
        {
            soundBankSoundEffects = sb;
        }

        public static void UpdateWaveBankMusic(WaveBank wb)
        {
            waveBankMusic = wb;
        }
        public static void UpdateSoundBankMusic(SoundBank sb)
        {
            soundBankMusic = sb;
        }

        public static void UpdateSong(string newSong)
        {
            soundBankMusic.PlayCue(newSong);
        }

        public static void UpdateParticleEngine(ParticleEngine pe)
        {
            particleEngine = pe;
        }

        public static void UpdateHUD(HUD hm)
        {
            hudManager = hm;
        }
        
    }

}
