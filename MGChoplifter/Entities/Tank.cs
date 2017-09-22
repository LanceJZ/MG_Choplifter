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
        public ThePlayer PlayerRef;
        TankTurret Turret;
        TankTred[] Treds = new TankTred[2];
        float MaxSpeed;
        float Seperation;
        float RightBound;

        public Tank(Game game, ThePlayer player) : base(game)
        {
            Turret = new TankTurret(game, player);
            PlayerRef = player;

            for (int i = 0; i < 2; i++)
            {
                Treds[i] = new TankTred(game);
                Treds[i].AddAsChild(this, true, false);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MaxSpeed = S.RandomMinMax(50, 100);
            Seperation = S.RandomMinMax(100, 200);
            RightBound = S.RandomMinMax(-1000, -1100);

            Turret.AddAsChild(this, true, false);
            Turret.Position.Y = 10;

            Treds[0].Position = new Vector3(0, -2, -16);
            Treds[1].Position = new Vector3(0, -2, 16);

        }

        public override void LoadContent()
        {
            SetModel(Game.Content.Load<XnaModel>("Models/CLTankBody"));
            Turret.LoadContent();
        }

        public override void BeginRun()
        {
            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (TankTred tred in Treds)
            {
                tred.Moving = false;
            }

            Velocity.X = 0;
            float differnceX = PlayerRef.Position.X - Position.X;

            if (differnceX > Seperation && PlayerRef.Position.X < RightBound)
            {
                Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);

                foreach (TankTred tred in Treds)
                {
                    tred.Moving = true;
                }
            }

            if (differnceX < -Seperation && PlayerRef.Position.X > PlayerRef.BoundLeftX)
            {
                Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);
                foreach (TankTred tred in Treds)
                {
                    tred.Moving = true;
                }
            }
        }
    }
}
