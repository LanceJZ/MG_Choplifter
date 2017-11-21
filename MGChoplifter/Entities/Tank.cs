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
    public class Tank : AModel
    {
        public ThePlayer PlayerRef;
        TankTurret Turret;
        TankTred[] Treds = new TankTred[2];
        float MaxSpeed;
        float Seperation;
        float RightBound;
        bool CollidedR;
        bool CollidedL;

        public Tank(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;
            Turret = new TankTurret(game, player);
            Active = false;

            for (int i = 0; i < 2; i++)
            {
                Treds[i] = new TankTred(game);
                Treds[i].AddAsChild(this, true, false);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MaxSpeed = Services.RandomMinMax(50, 100);
            Seperation = Services.RandomMinMax(100, 200);
            RightBound = Services.RandomMinMax(-1000, -1100);

            Turret.AddAsChild(this, true, false);
            Position.Y = -280;
            Radius = 24;
            LoadContent();
        }

        public override void LoadContent()
        {
            SetModel(Game.Content.Load<XnaModel>("Models/CLTankBody"));
            BeginRun();
        }

        public override void BeginRun()
        {
            Turret.Position.Y = 10;

            Treds[0].Position = new Vector3(0, -2, -16);
            Treds[1].Position = new Vector3(0, -2, 16);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (TankTred tred in Treds)
            {
                tred.Moving = false;
            }

            FollowPlayer();

        }

        public void Spawn()
        {
            Position.X = Services.RandomMinMax(-3000, -8000);
            Active = true;
        }

        public void BumpedR()
        {
            CollidedR = true;
        }

        public void BumpedL()
        {
            CollidedL = true;
        }

        public void NotBumped()
        {
            CollidedL = false;
            CollidedR = false;
        }

        void FollowPlayer()
        {
            Velocity.X = 0;

            float differnceX = PlayerRef.Position.X - Position.X;

            if (differnceX > Seperation && PlayerRef.Position.X < RightBound && !CollidedR)
            {
                Velocity.X = MathHelper.Clamp(differnceX * 0.1f, -MaxSpeed, MaxSpeed);

                foreach (TankTred tred in Treds)
                {
                    tred.Moving = true;
                }
            }

            if (differnceX < -Seperation && PlayerRef.Position.X > PlayerRef.BoundLeftX && !CollidedL)
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
