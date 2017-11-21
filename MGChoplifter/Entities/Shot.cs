using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using Sys = Engine.Services;
    using Time = Engine.Timer;
    using Mod = Engine.AModel;

    public class Shot : Mod
    {
        Time LifeTimer;

        public Shot(Game game, XnaModel model) : base(game, model)
        {
            LifeTimer = new Time(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Active = false;
            Radius = 2;
            Scale = 2;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (LifeTimer.Expired)
                Active = false;
        }

        public void Spawn(Vector3 postion, Vector2 direction, float timer)
        {
            Active = true;
            Position = postion;
            Velocity = new Vector3(direction.X, direction.Y, 0);
            Vector3 acc = Acceleration;
            LifeTimer.Reset(timer);
        }
    }
}
