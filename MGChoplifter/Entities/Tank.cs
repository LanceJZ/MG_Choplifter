using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;

    public class Tank : Engine.AModel
    {
        TankTurret Turret;
        Engine.AModel[] TredAnimationsL = new Engine.AModel[2];
        Engine.AModel[] TredAnimationsR = new Engine.AModel[2];
        T AnimationTimer;

        public Tank(Game game) : base(game)
        {
            Turret = new TankTurret(game);

            for (int i = 0; i < 2; i++)
            {
                TredAnimationsL[i] = new Engine.AModel(game);
                TredAnimationsL[i].AddAsChild(this, true, false);
                TredAnimationsR[i] = new Engine.AModel(game);
                TredAnimationsR[i].AddAsChild(this, true, false);
            }

            AnimationTimer = new T(game, 0.1f);
        }

        public override void Initialize()
        {
            base.Initialize();

            Turret.Position.Y = 10;

            for (int i = 0; i < 2; i++)
            {
                TredAnimationsL[i].Position = new Vector3(0, -2, -16);
                TredAnimationsR[i].Position = new Vector3(0, -2, 16);
            }

            Turret.AddAsChild(this, true, false);
        }

        public void LoadContent()
        {
            LoadModel(Game.Content.Load<XnaModel>("Models/CLTankBody"), null);
            Turret.LoadContent();

            TredAnimationsL[0].LoadModel(Game.Content.Load<XnaModel>("Models/CLTankTred1"), null);
            TredAnimationsL[1].LoadModel(Game.Content.Load<XnaModel>("Models/CLTankTred2"), null);
            TredAnimationsR[0].LoadModel(Game.Content.Load<XnaModel>("Models/CLTankTred1"), null);
            TredAnimationsR[1].LoadModel(Game.Content.Load<XnaModel>("Models/CLTankTred2"), null);

        }

        public override void BeginRun()
        {
            base.BeginRun();

            TredAnimationsL[0].Visable = false;
            TredAnimationsR[1].Visable = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (AnimationTimer.Expired)
            {
                AnimationTimer.Reset();

                for (int i = 0; i < 2; i++)
                {
                    TredAnimationsL[i].Visable = !TredAnimationsL[i].Visable;
                    TredAnimationsR[i].Visable = !TredAnimationsL[i].Visable;
                }
            }
        }
    }
}
