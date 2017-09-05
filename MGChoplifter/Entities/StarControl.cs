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

    public class StarControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent
    {
        Engine.AModel[] Stars = new Engine.AModel[50];
        float[] StarsX;

        public StarControl(Game game) : base(game)
        {
            StarsX = new float[Stars.Length];

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i] = new Engine.AModel(game, Game.Content.Load<XnaModel>("Models/cube"), null);
            }

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].Position = new Vector3(S.RandomMinMax(-600, 600), S.RandomMinMax(-200, 400), -300);
                StarsX[i] = Stars[i].Position.X;
                Stars[i].RotationVelocity = new Vector3(S.RandomMinMax(-10, 10), S.RandomMinMax(-10, 10),
                    S.RandomMinMax(-10, 10));
            }

            S.AddBeginable(this);
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
