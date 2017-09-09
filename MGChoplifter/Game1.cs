using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;

namespace MGChoplifter
{
    using PO = Engine.PositionedObject;
    using S = Engine.Services;
    using E = Entities;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager GraphicsDM;
        SpriteBatch spriteBatch;

        Engine.AModel cubeTest;

        List<Engine.AModel> ModelTest = new List<Engine.AModel>();

        E.ThePlayer Player;
        E.HouseControl Houses;
        E.Background Background;
        E.EnemyControl Enemies;

        public Game1()
        {
            for (int i = 0; i < 17; i++)
            {
                //ModelTest.Add(new Engine.AModel(this));
            }

            cubeTest = new Engine.AModel(this);

            GraphicsDM = new GraphicsDeviceManager(this);
            GraphicsDM.IsFullScreen = false;
            GraphicsDM.SynchronizeWithVerticalRetrace = true;
            GraphicsDM.GraphicsProfile = GraphicsProfile.HiDef;
            GraphicsDM.PreferredBackBufferWidth = 1200;
            GraphicsDM.PreferredBackBufferHeight = 900;
            GraphicsDM.PreferMultiSampling = true; //Error in MonoGame 3.6 for DirectX, fixed for dev version.
            GraphicsDM.PreparingDeviceSettings += SetMultiSampling;
            GraphicsDM.ApplyChanges();
            GraphicsDM.GraphicsDevice.RasterizerState = new RasterizerState();
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            Player = new E.ThePlayer(this);
            Houses = new E.HouseControl(this);
            Background = new E.Background(this, Player);
            Enemies = new E.EnemyControl(this, Player);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 4;
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
            S.Initialize(GraphicsDM, this, new Vector3(0, 0, 200), 0, 1000);
            // Setup lighting.
            S.DefuseLight = new Vector3(0.6f, 0.5f, 0.7f);
            S.LightDirection = new Vector3(-0.75f, -0.75f, -0.5f);
            S.SpecularColor = new Vector3(0.1f, 0, 0.5f);
            S.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f); // Add some overall ambient light.

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //cubeTest.LoadModel(Content.Load<XnaModel>("Models/Cube"), null);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (ModelTest.Count < 16)
                return;

            ModelTest[0].LoadModel(Content.Load<XnaModel>("Models/CLHouse"), null);
            ModelTest[1].LoadModel(Content.Load<XnaModel>("Models/CLPlayerChopper"), null);
            ModelTest[2].LoadModel(Content.Load<XnaModel>("Models/CLTankBody"), null);
            ModelTest[3].LoadModel(Content.Load<XnaModel>("Models/CLBarakade"), null);
            ModelTest[4].LoadModel(Content.Load<XnaModel>("Models/CLBaseV2"), null);
            ModelTest[5].LoadModel(Content.Load<XnaModel>("Models/CLHouseOpen"), null);
            ModelTest[6].LoadModel(Content.Load<XnaModel>("Models/CLPersonArm"), null);
            ModelTest[7].LoadModel(Content.Load<XnaModel>("Models/CLPersonLeg"), null);
            ModelTest[8].LoadModel(Content.Load<XnaModel>("Models/CLPersonMan"), null);
            ModelTest[9].LoadModel(Content.Load<XnaModel>("Models/CLPlayerMainBlade"), null);
            ModelTest[10].LoadModel(Content.Load<XnaModel>("Models/CLPlayerRotor"), null);
            ModelTest[11].LoadModel(Content.Load<XnaModel>("Models/CLTankBarral"), null);
            ModelTest[12].LoadModel(Content.Load<XnaModel>("Models/CLTankTred1"), null);
            ModelTest[13].LoadModel(Content.Load<XnaModel>("Models/CLTankTred2"), null);
            ModelTest[14].LoadModel(Content.Load<XnaModel>("Models/CLTankTurret"), null);
            ModelTest[15].LoadModel(Content.Load<XnaModel>("Models/Mountain"), null);
        }

        protected override void BeginRun()
        {
            base.BeginRun();

            cubeTest.Scale = 10;

            S.BeginRun();

            float posx = 0;
            float posy = 0;

            foreach (Engine.AModel mod in ModelTest)
            {
                mod.RotationVelocity = new Vector3(0, 0.5f, 0);
                mod.Position = new Vector3(-360 + posx, -230 + posy, 0);

                posx += 45;
                posy += 28;
            }

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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);



            base.Draw(gameTime);
        }

    }
}
