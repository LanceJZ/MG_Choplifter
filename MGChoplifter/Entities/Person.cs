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
    using Mod = AModel;

    public class Person : Mod
    {
        enum CurrentState
        {
            Running,
            Waving
        };

        enum CurrentMode
        {
            Waiting,
            DroppedOff,
            Enter
        };

        CurrentState State;
        CurrentMode Mode;
        Timer Attention;
        ThePlayer PlayerRef;
        public Mod[] Arms = new Mod[2];
        public Mod[] Legs = new Mod[2];
        float Seperation;
        float MaxSpeed;
        float RightBound;

        public Person(Game game, ThePlayer player, XnaModel model) : base(game, model)
        {
            PlayerRef = player;
            Attention = new Timer(game, Services.RandomMinMax(0.25f, 2));
        }

        public override void Initialize()
        {
            Radius = 10;

            Position.Z = Services.RandomMinMax(-49, -1);
            Seperation = Services.RandomNumber.Next(20, 200);
            MaxSpeed = Services.RandomNumber.Next(10, 25);
            RightBound = Services.RandomMinMax(-10000, -9000);

            for (int i = 0; i < 2; i++)
            {
                Arms[i] = new Mod(Game);
                Legs[i] = new Mod(Game);
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

            Active = false;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                switch (Mode)
                {
                    case CurrentMode.Waiting:
                        if (Attention.Expired)
                        {
                            Attention.Reset();
                            ChaseOrWave();
                        }
                        break;

                    case CurrentMode.DroppedOff:
                        RunToBase();
                        break;

                    case CurrentMode.Enter:
                        Entering();
                        break;
                }

                switch (State)
                {
                    case CurrentState.Running:
                        Running();
                        break;

                    case CurrentState.Waving:
                        Waving();
                        break;
                }
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position, bool dropped)
        {
            Position.Y = position.Y;
            Active = true;

            if (dropped)
            {
                Mode = CurrentMode.DroppedOff;
                Position.X = position.X;
                SwitchToRunning();
            }
            else
            {
                Mode = CurrentMode.Waiting;
                Position.X = position.X + Services.RandomMinMax(-20, 20);
                SwitchToWaving();
            }
        }

        void Entering()
        {
            if (PlayerRef.Position.Y > PlayerRef.BoundLowY + 10)
            {
                Mode = CurrentMode.Waiting;
                State = CurrentState.Waving;
                return;
            }

            if(State == CurrentState.Waving)
            {
                SwitchToRunning();
            }

            if (Position.X > PlayerRef.Position.X)
                Velocity.X = -MaxSpeed;
            else
                Velocity.X = MaxSpeed;

            if (CirclesIntersect(PlayerRef))
            {
                if (PlayerRef.PickUpPerson())
                {
                    Active = false;
                }
                else
                {
                    Mode = CurrentMode.Waiting;
                    State = CurrentState.Waving;
                }
            }
        }

        void RunToBase()
        {
            if (State == CurrentState.Waving)
            {
                SwitchToRunning();
            }

            Velocity.X = MaxSpeed;

            if (Position.X > 144.5f)
                Active = false;
        }

        void ChaseOrWave()
        {
            Velocity.X = 0;

            if (PlayerRef.Position.X > RightBound)
            {
                if (PlayerRef.Position.Y < PlayerRef.BoundLowY + 10)
                {
                    Mode = CurrentMode.Enter;
                    return;
                }

                float differnceX = PlayerRef.Position.X - Position.X;

                if (differnceX > Seperation)
                {
                    Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                    }
                }
                else if (differnceX < -Seperation)
                {
                    Velocity.X = MathHelper.Clamp(differnceX, -MaxSpeed, MaxSpeed);

                    if (State == CurrentState.Waving)
                    {
                        SwitchToRunning();
                    }
                }
            }
            else
            {
                if (State == CurrentState.Running)
                {
                    SwitchToWaving();
                }

            }
        }

        void SwitchToRunning()
        {
            Arms[0].Rotation.X = -MathHelper.PiOver2;
            Arms[1].Rotation.X = MathHelper.PiOver2;
            Legs[0].RotationVelocity.X = MathHelper.Pi;
            Legs[1].RotationVelocity.X = -MathHelper.Pi;
            Rotation.Y = MathHelper.PiOver2;
            State = CurrentState.Running;
        }

        void SwitchToWaving()
        {
            Arms[0].Rotation.X = MathHelper.Pi - MathHelper.PiOver4;
            Arms[1].Rotation.X = MathHelper.Pi + MathHelper.PiOver4;
            Arms[0].RotationVelocity.X = MathHelper.Pi;
            Arms[1].RotationVelocity.X = -MathHelper.Pi;
            Rotation.Y = 0;
            State = CurrentState.Waving;
        }

        void Running()
        {
            foreach(Mod arm in Arms)
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

            foreach (Mod leg in Legs)
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
            foreach (Mod leg in Legs)
            {
                leg.Rotation.X = 0;
                leg.RotationVelocity.X = 0;
            }

            foreach (Mod arm in Arms)
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
