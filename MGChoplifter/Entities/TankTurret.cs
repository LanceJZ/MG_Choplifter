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

    public class TankTurret : Engine.AModel
    {
        Engine.AModel Barral;

        public TankTurret(Game game) : base(game)
        {
            Barral = new Engine.AModel(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Barral.AddAsChild(this, true, false);
        }

        public void LoadContent()
        {
            LoadModel(Game.Content.Load<XnaModel>("Models/CLTankTurret"), null);
            Barral.LoadModel(Game.Content.Load<XnaModel>("Models/CLTankBarral"), null);
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Barral.Position.X = 12;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


        }
    }
}
