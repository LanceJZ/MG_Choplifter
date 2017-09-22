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

    public class Person : Engine.AModel
    {
        enum CurrentState
        {
            Running,
            Waving
        };

        CurrentState Doing;

        public Engine.AModel[] Arms = new Engine.AModel[2];
        public Engine.AModel[] Legs = new Engine.AModel[2];
        float Seperation;
        float MaxSpeed;
        float RightBound;
        ThePlayer PlayerRef;

        public Person(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

        }

        public override void Initialize()
        {
            base.Initialize();

            Position.Z = S.RandomMinMax(-90, -20);
            Seperation = S.RandomNumber.Next(20, 200);
            MaxSpeed = S.RandomNumber.Next(10, 25);
            RightBound = S.RandomMinMax(-10000, -9000);

            for (int i = 0; i < 2; i++)
            {
                Arms[i] = new Engine.AModel(Game);
                Legs[i] = new Engine.AModel(Game);
            }

            for (int i = 0; i < 2; i++)
            {
                Arms[i].AddAsChild(this, true, false);
                Legs[i].AddAsChild(this, true, false);

                Arms[i].Position.Y = 7;
                Legs[i].Position.Y = 0;
            }

            Arms[0].Position.X = -2;
            Arms[1].Position.X = 2;

            Legs[0].Position.X = -1;
            Legs[1].Position.X = 1;

            Doing = CurrentState.Waving;
        }

        public override void LoadContent()
        {

        }

        public override void BeginRun()
        {
            base.BeginRun();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Active)
            {
                ChaseOrWave();

                switch (Doing)
                {
                    case CurrentState.Running:
                        Running();
                        break;

                    case CurrentState.Waving:
                        Waving();
                        break;
                }
            }
        }

        public void Spawn(Vector3 position)
        {
            Position.X = position.X + S.RandomMinMax(-20, 20);
            Position.Y = position.Y;
            Active = true;
        }

        void ChaseOrWave()
        {
            Velocity.X = 0;
            float differnceX = PlayerRef.Position.X - Position.X;

            if (differnceX > Seperation)// && PlayerRef.Position.X < RightBound)
            {
                Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                if (Doing == CurrentState.Waving)
                {
                    Arms[0].Rotation.X = -MathHelper.PiOver2;
                    Arms[1].Rotation.X = MathHelper.PiOver2;
                    Legs[0].RotationVelocity.X = MathHelper.Pi;
                    Legs[1].RotationVelocity.X = -MathHelper.Pi;
                }

                Doing = CurrentState.Running;
            }
            else if (differnceX < -Seperation && PlayerRef.Position.X > PlayerRef.BoundLeftX)
            {
                Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                if (Doing == CurrentState.Waving)
                {
                    Arms[0].Rotation.X = -MathHelper.PiOver2;
                    Arms[1].Rotation.X = MathHelper.PiOver2;
                    Legs[0].RotationVelocity.X = MathHelper.Pi;
                    Legs[1].RotationVelocity.X = -MathHelper.Pi;
                }

                Doing = CurrentState.Running;
            }
            else
            {
                if (Doing == CurrentState.Running)
                {
                    Arms[0].Rotation.X = MathHelper.Pi - MathHelper.PiOver4;
                    Arms[1].Rotation.X = MathHelper.Pi + MathHelper.PiOver4;
                    Arms[0].RotationVelocity.X = MathHelper.Pi;
                    Arms[1].RotationVelocity.X = -MathHelper.Pi;
                }

                Doing = CurrentState.Waving;
            }
        }

        void Running()
        {
            Rotation.Y = MathHelper.PiOver2;

            foreach(Engine.AModel arm in Arms)
            {
                if (arm.Rotation.X < -MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = MathHelper.Pi;
                }

                if (arm.Rotation.X > MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = -MathHelper.Pi;
                }
            }

            foreach (Engine.AModel leg in Legs)
            {
                if (leg.Rotation.X < -MathHelper.PiOver4)
                {
                    leg.RotationVelocity.X = MathHelper.Pi;
                }

                if (leg.Rotation.X > MathHelper.PiOver4)
                {
                    leg.RotationVelocity.X = -MathHelper.Pi;
                }
            }
        }

        void Waving()
        {
            Rotation.Y = 0;

            foreach (Engine.AModel leg in Legs)
            {
                leg.Rotation.X = 0;
                leg.RotationVelocity.X = 0;
            }

            foreach (Engine.AModel arm in Arms)
            {
                if (arm.Rotation.X < MathHelper.Pi - MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = MathHelper.Pi;
                }

                if (arm.Rotation.X > MathHelper.Pi + MathHelper.PiOver4)
                {
                    arm.RotationVelocity.X = -MathHelper.Pi;
                }
            }
        }
    }
}
