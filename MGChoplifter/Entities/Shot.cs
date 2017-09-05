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

    public class Shot : Engine.AModel
    {
        T LifeTimer;

        public Shot(Game game) : base(game)
        {
            LifeTimer = new T(game, 2.2f);
        }

        public override void Initialize()
        {
            base.Initialize();

            LifeTimer.Initialize();

            Radius = 1;
        }

        public override void BeginRun()
        {
            base.BeginRun();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (LifeTimer.Expired)
                Active = false;
        }

        public void LoadModel(XnaModel model)
        {
            LoadModel(model, null);
        }

        public void Spawn(Vector3 postion, Vector2 direction)
        {
            Active = true;
            Position = postion;
            Velocity = new Vector3(direction.X, direction.Y, 0);
            LifeTimer.Reset();
        }
    }
}
