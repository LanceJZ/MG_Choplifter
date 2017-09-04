﻿#region Using
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Engine
{
    public sealed class Services : DrawableGameComponent
    {
        #region Fields
        private static Services m_Instance = null;
        private static Random m_RandomNumber;
        private static SpriteBatch m_SpriteBatch;
        private static GraphicsDeviceManager m_GraphicsDM;
        private static Camera camera;
        private static Vector3 m_DefuseLight = Vector3.One;
        private static Vector3 m_LightDirection = Vector3.Left;
        private static Vector3 m_SpecularColor = Vector3.Zero;
        private static Vector3 m_AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f);
        private static Vector3 m_EmissivieColor = Vector3.Zero;
        private static List<IDrawComponent> m_DrawableComponents;
        private static List<IUpdateableComponent> m_UpdateableComponents;
        private static List<IBeginable> m_Beginable;
        #endregion
        #region Properties
        /// <summary>
        /// This is used to get the Services Instance
        /// Instead of using the mInstance this will do the check to see if the Instance is valid
        /// where ever you use it. It is also private so it will only get used inside the engine services.
        /// </summary>
        private static Services Instance
        {
            get
            {
                //Make sure the Instance is valid
                if (m_Instance != null)
                {
                    return m_Instance;
                }

                throw new InvalidOperationException("The Engine Services have not been started!");
            }
        }

        public static GraphicsDeviceManager GraphicsDM { get => m_GraphicsDM; }
        public static SpriteBatch SpriteBatch { get => m_SpriteBatch; set => m_SpriteBatch = value; }
        public static Camera Camera { get => camera; }
        public static Random RandomNumber { get => m_RandomNumber; }
        /// <summary>
        /// Returns the window size in pixels, of the height.
        /// </summary>
        /// <returns>int</returns>
        public static int WindowHeight { get => m_GraphicsDM.PreferredBackBufferHeight; }
        /// <summary>
        /// Returns the window size in pixels, of the width.
        /// </summary>
        /// <returns>int</returns>
        public static int WindowWidth { get => m_GraphicsDM.PreferredBackBufferWidth; }

        public static Vector2 WindowSize
        {
            get => new Vector2(m_GraphicsDM.PreferredBackBufferWidth, m_GraphicsDM.PreferredBackBufferHeight);
        }

        public static Vector3 DefuseLight { get => m_DefuseLight; set => m_DefuseLight = value; }
        public static Vector3 LightDirection { get => m_LightDirection; set => m_LightDirection = value; }
        public static Vector3 SpecularColor { get => m_SpecularColor; set => m_SpecularColor = value; }
        public static Vector3 AmbientLightColor { get => m_AmbientLightColor; set => m_AmbientLightColor = value; }
        public static Vector3 EmissivieColor { get => m_EmissivieColor; set => m_EmissivieColor = value; }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the constructor for the Services
        /// You will note that it is private that means that only the Services can only create itself.
        /// </summary>
        private Services(Game game) : base(game)
        {
            game.Components.Add(this);
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }
        #endregion
        #region Public Methods
        protected override void LoadContent()
        {
            foreach(IDrawComponent loadable in m_DrawableComponents)
            {
                //loadable.LoadContent();
            }
        }

        public static void BeginRun()
        {
            foreach(IBeginable begin in m_Beginable)
            {
                begin.BeginRun();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //}

            foreach (IDrawComponent drawable in m_DrawableComponents)
            {
                drawable.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (IUpdateableComponent updateable in m_UpdateableComponents)
            {
                updateable.Update(gameTime);
            }
        }
        /// <summary>
        /// This is used to start up Panther Engine Services.
        /// It makes sure that it has not already been started if it has been it will throw and exception
        /// to let the user know.
        ///
        /// You pass in the game class so you can get information needed.
        /// </summary>
        /// <param name="graphics">Reference to the graphic device.</param>
        public static void Initialize(GraphicsDeviceManager graphics, Game game, Vector3 CameraPosition, float near, float far)
        {
            //First make sure there is not already an instance started
            if (m_Instance == null)
            {
                m_GraphicsDM = graphics;
                //Create the Engine Services
                m_Instance = new Services(game);
                m_RandomNumber = new Random(DateTime.Now.Millisecond);
                camera = new Camera(game, CameraPosition, Vector3.Zero, Vector3.Zero, true, near, far);
                //BasicEffect = new BasicEffect(graphics.GraphicsDevice);
                //BasicEffect.LightingEnabled = true;
                //BasicEffect.View = m_ViewMatrix;
                //BasicEffect.Projection = m_ProjectionMatrix;
                //BasicEffect.World = WorldMatrix;
                //m_WorldMatrix = Matrix.CreateTranslation(Vector3.Zero);
                m_DrawableComponents = new List<IDrawComponent>();
                m_UpdateableComponents = new List<IUpdateableComponent>();
                m_Beginable = new List<IBeginable>();

                return;
            }

            throw new Exception("The Engine Services have already been started.");
        }
        /// <summary>
        /// Get a random float between min and max
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        public static float RandomMinMax(float min, float max)
        {
            return min + (float)RandomNumber.NextDouble() * (max - min);
        }

        public static void AddDrawableComponent(IDrawComponent drawableComponent)
        {
            m_DrawableComponents.Add(drawableComponent);
        }

        public static void AddUpdateableComponent(IUpdateableComponent updateableComponent)
        {
            m_UpdateableComponents.Add(updateableComponent);
        }

        public static void AddBeginable(IBeginable beginable)
        {
            m_Beginable.Add(beginable);
        }

        #endregion
    }
}
