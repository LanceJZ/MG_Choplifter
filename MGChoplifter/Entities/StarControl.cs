using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class StarControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent, Engine.ILoadContent
    {
        Engine.AModel[] Stars = new Engine.AModel[50];
        float[] StarsX;

        public StarControl(Game game) : base(game)
        {
            StarsX = new float[Stars.Length];

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            float spinSpeed = 7.5f;

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i] = new Engine.AModel(Game);
            }

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].Position = new Vector3(S.RandomMinMax(-600, 600), S.RandomMinMax(-200, 400), -300);
                Stars[i].RotationVelocity = new Vector3(S.RandomMinMax(-spinSpeed, spinSpeed),
                    S.RandomMinMax(-spinSpeed, spinSpeed), S.RandomMinMax(-spinSpeed, spinSpeed));
                Stars[i].Scale = S.RandomNumber.Next(1, 3);
                StarsX[i] = Stars[i].Position.X;
            }

            S.AddBeginable(this);
            S.AddLoadable(this);
        }

        public void LoadContent()
        {
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].LoadModel("cube");
            }
        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].Position.X = StarsX[i] + S.Camera.Position.X;
            }
        }
    }
}
