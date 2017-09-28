using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using Engine;

namespace MGChoplifter.Entities
{
    using Sys = Services;
    using Time = Timer;
    using Mod = AModel;

    public class TankTred : PositionedObject, ILoadContent
    {
        Mod[] TredAnimations = new Mod[2];
        Time AnimationTimer;

        public bool Moving;

        public TankTred(Game game) : base(game)
        {
            for (int i = 0; i < 2; i++)
            {
                TredAnimations[i] = new Mod(game);
                TredAnimations[i].AddAsChild(this, true, false);
            }

            AnimationTimer = new Time(game, 0.1f);
        }

        public override void Initialize()
        {
            base.Initialize();

            Sys.AddLoadable(this);
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
