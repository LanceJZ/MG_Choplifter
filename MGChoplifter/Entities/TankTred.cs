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

    public class TankTred : Engine.PositionedObject, Engine.ILoadContent
    {
        Engine.AModel[] TredAnimations = new Engine.AModel[2];
        T AnimationTimer;

        public bool Moving;

        public TankTred(Game game) : base(game)
        {
            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i] = new Engine.AModel(game);
                TredAnimations[i].AddAsChild(this, true, false);
            }

            AnimationTimer = new T(game, 0.1f);
        }

        public override void Initialize()
        {
            base.Initialize();

            S.AddLoadable(this);
        }

        public void LoadContent()
        {
            TredAnimations[0].SetModel(Game.Content.Load<XnaModel>("Models/CLTankTred1"));
            TredAnimations[1].SetModel(Game.Content.Load<XnaModel>("Models/CLTankTred2"));
        }

        public override void BeginRun()
        {
            base.BeginRun();

            TredAnimations[0].Visable = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (AnimationTimer.Expired && Moving)
            {
                AnimationTimer.Reset();

                for(int i = 0; i < 2; i++)
                {
                    TredAnimations[i].Visable = !TredAnimations[i].Visable;
                }
            }
        }
    }
}
