using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using MGChoplifter.Entities;
using Engine;

namespace MGChoplifter
{
    using PO = PositionedObject;
    using GameServices = Services;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        uint FramesPS = 0;
        uint TotalFrames = 0;
        uint FrameSeconds = 0;

        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        Timer FPSTimer;

        ThePlayer Player;
        HouseControl Houses;
        Background Background;
        EnemyControl Enemies;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferWidth = 1200;
            Graphics.PreferredBackBufferHeight = 900;
            Graphics.PreferMultiSampling = true; //Error in MonoGame 3.6 for DirectX, fixed for dev version.
            Graphics.PreparingDeviceSettings += SetMultiSampling;
            Graphics.ApplyChanges();
            Graphics.GraphicsDevice.RasterizerState = new RasterizerState(); //Must be after Apply Changes.
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            FPSTimer = new Timer(this, 1);

            Player = new ThePlayer(this);
            Background = new Background(this, Player);
            Enemies = new EnemyControl(this, Player);
            Houses = new HouseControl(this, Player);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 8;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Positive Y is Up. Positive X is Right.
            GameServices.Initialize(Graphics, this, new Vector3(0, 0, 200), 0, 1000);
            // Setup lighting.
            GameServices.DefuseLight = new Vector3(0.6f, 0.5f, 0.7f);
            GameServices.LightDirection = new Vector3(-0.75f, -0.75f, -0.5f);
            GameServices.SpecularColor = new Vector3(0.1f, 0, 0.5f);
            GameServices.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f); // Add some overall ambient light.

            base.Initialize();
        }

        protected override void BeginRun()
        {
            GameServices.BeginRun(); //This only happens once in a game.

            base.BeginRun();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            FramesPS++;

            if (FPSTimer.Expired)
            {
                FPSTimer.Reset();
                TotalFrames += FramesPS;
                FrameSeconds++;
                float average = TotalFrames / FrameSeconds;
                System.Diagnostics.Debug.WriteLine("FPS " + FramesPS.ToString() + " Average " + average.ToString());
                FramesPS = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(5, 0, 40));

            base.Draw(gameTime);
        }

    }
}
