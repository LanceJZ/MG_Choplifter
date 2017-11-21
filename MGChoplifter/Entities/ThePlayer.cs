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

    public class ThePlayer : Mod
    {
        enum Direction
        {
            Right,
            ForwardFromRight,
            Left,
            ForwardFromLeft
        };

        enum CurrentState
        {
            Unload,
            Load,
            Flight
        };

        public Shot[] Shots = new Shot[5];
        public Person[] People = new Person[4];

        Mod MainBlade;
        Mod Rotor;

        Timer FireTimer;
        Timer TurnTimer;
        Timer UnloadTimer;

        KeyboardState KeyState;
        KeyboardState KeyStateOld;

        public XnaModel PersonMan;
        public XnaModel PersonLeg;
        public XnaModel PersonArm;
        public float BoundLeftX = -6600 * 2;
        public float BoundLowY = -219.5f;
        float AccelerationAmount = 220;
        float MaxSpeed = 650;
        float Tilt = MathHelper.PiOver4 / 12f;
        float UnloadRate = 1.5f;
        float FireRate = 0.1f;
        float TurnRate = 1.1f;
        float RotateRate = MathHelper.PiOver2;
        float MoveHorizontal;
        float BoundHighY = 364.25f;
        float BoundRightX = 145.5f;
        float UnloadX = 10.5f;
        int ShotLimit = 5;
        int NumberOfPassengers;
        int PassengerLimit = 4;
        bool FacingChanged;
        bool Coasting;

        Direction Facing;
        CurrentState State;

        public ThePlayer(Game game) : base(game)
        {
            MainBlade = new Mod(game);
            Rotor = new Mod(game);
            FireTimer = new Timer(game, FireRate);
            TurnTimer = new Timer(game, TurnRate);
            UnloadTimer = new Timer(game, UnloadRate);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);

            Radius = 26;
            Facing = Direction.Right;
            State = CurrentState.Flight;
            NumberOfPassengers = 0; //TODO: For testing. Should be zero.

            base.Initialize();
        }

        public override void LoadContent()
        {
            MainBlade.LoadModel("CLPlayerMainBlade");
            Rotor.LoadModel("CLPlayerRotor");
            LoadModel("CLPlayerChopper");

            Shots[0] = new Shot(Game, Load("cube"));
        }

        public override void BeginRun()
        {
            MainBlade.AddAsChild(this, true, false);
            Rotor.AddAsChild(this, true, false);

            MainBlade.Position = new Vector3(0, 12f, 0);
            MainBlade.RotationVelocity = new Vector3(0, 20, 0);
            Rotor.Position = new Vector3(-26, 8, -2);
            Rotor.RotationVelocity = new Vector3(0, 0, 24);

            for (int i = 0; i < People.Length; i++)
            {
                People[i] = new Person(Game, this, PersonMan);

                for (int ii = 0; ii < 2; ii++)
                {
                    People[i].Arms[ii].SetModel(PersonArm);
                    People[i].Legs[ii].SetModel(PersonLeg);
                }
            }

            for (int i = 1; i < ShotLimit; i++)
            {
                Shots[i] = new Shot(Game, Shots[0].XNAModel);
            }

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {

            GetInput();
            TeltChopper();
            CheckToUnload();

            if (Position.X < BoundLeftX || Position.X > BoundRightX || Position.Y > BoundHighY || Position.Y < BoundLowY)
            {
                StopMovementX();
                StopMovementY();
            }

            if (FacingChanged)
                UpdateFacing();

            if (Coasting)
                WindResistance();

            base.Update(gameTime);
            Services.Camera.Position.X = Position.X;
        }

        public bool PickUpPerson()
        {
            if (NumberOfPassengers == PassengerLimit)
                return false;

            NumberOfPassengers++;
                return true;
        }

        void CheckToUnload()
        {
            if (Position.Y < -215 && Velocity == Vector3.Zero && Position.X > UnloadX)
            {
                if (State == CurrentState.Flight)
                    UnloadTimer.Reset();

                State = CurrentState.Unload;
            }
            else
            {
                State = CurrentState.Flight;
            }

            switch (State)
            {
                case CurrentState.Unload:
                    Unload();
                    break;
            }
        }

        void Unload()
        {
            if (UnloadTimer.Expired)
            {
                UnloadTimer.Reset();

                if (NumberOfPassengers > 0)
                {
                    Vector3 pos = new Vector3(Position.X, BoundLowY - 5, People[NumberOfPassengers - 1].Position.Z);
                    People[NumberOfPassengers - 1].Spawn(pos, true);
                    NumberOfPassengers--;
                }
            }
        }

        void GetInput()
        {
            KeyState = Keyboard.GetState();
            //Check keys after this. --------------
            if (KeyState.IsKeyDown(Keys.RightControl) && !KeyStateOld.IsKeyDown(Keys.RightControl))
            {
                if (TurnTimer.Expired)
                {
                    TurnTimer.Reset();

                    ChangeFacing();
                }
            }

            if (KeyState.IsKeyDown(Keys.LeftControl) && !KeyStateOld.IsKeyDown(Keys.LeftControl))
            {
                if (FireTimer.Expired)
                {
                    FireTimer.Reset();

                    FireShot();
                }
            }

            Acceleration = Vector3.Zero;
            MoveHorizontal = 0;
            Coasting = true;

            if (KeyState.IsKeyDown(Keys.Left))
            {

                if (Position.X > BoundLeftX)
                {
                    MoveHorizontal = 1;
                    MoveX(new Vector3(-AccelerationAmount, 0, 0));
                    CheckPositionX();
                    Coasting = false;
                }
                else
                {
                    StopMovementX();
                }
            }
            else if (KeyState.IsKeyDown(Keys.Right))
            {

                if (Position.X < BoundRightX)
                {
                    MoveHorizontal = -1;
                    MoveX(new Vector3(AccelerationAmount, 0, 0));
                    CheckPositionX();
                    Coasting = false;
                }
                else
                {
                    StopMovementX();
                }
            }

            if (KeyState.IsKeyDown(Keys.Up))
            {

                if (Position.Y < BoundHighY)
                {
                    MoveY(new Vector3(0, AccelerationAmount * 0.2f, 0));

                    CheckPositionY();
                    Coasting = false;
                }
                else
                {
                    StopMovementY();
                }
            }
            else if (KeyState.IsKeyDown(Keys.Down))
            {

                if (Position.Y > BoundLowY)
                {
                    MoveY(new Vector3(0, -AccelerationAmount * 0.4f, 0));

                    CheckPositionY();
                    Coasting = false;
                }
                else
                {
                    StopMovementY();
                }
            }

            //Check keys before this. ----------------
            KeyStateOld = KeyState;
        }

        void FireShot()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!Shots[i].Active)
                {
                    Vector3 pos = Position;
                    Vector2 vel = Vector2.Zero;

                    switch (Facing)
                    {
                        case Direction.Right:
                            vel = SetVelocityFromAngle(MathHelper.Clamp(Rotation.Z, -MathHelper.PiOver4,
                                MathHelper.PiOver4), Velocity.X + 400);
                            pos.X += 20;
                            break;

                        case Direction.Left:
                            vel = SetVelocityFromAngle(MathHelper.Clamp(-Rotation.Z, -MathHelper.PiOver4,
                                MathHelper.PiOver4), Velocity.X - 400);
                            pos.X -= 20;
                            break;

                        case Direction.ForwardFromRight:
                        case Direction.ForwardFromLeft:
                            vel.Y = -200;
                            pos.Y -= 20;
                            break;
                    }

                    Shots[i].Spawn(pos, vel, 1.5f);
                    break;
                }
            }
        }

        void WindResistance()
        {
            float Deceration = 1.15f;
            Acceleration = -Velocity * Deceration;
        }

        void StopMovementX()
        {
            Velocity.X = 0;
            Acceleration.X = 0;
        }

        void StopMovementY()
        {
            Velocity.Y = 0;
            Acceleration.Y = 0;
        }

        void CheckPositionX()
        {
            Position.X = MathHelper.Clamp(Position.X, BoundLeftX, BoundRightX);
        }

        void CheckPositionY()
        {
            Position.Y = MathHelper.Clamp(Position.Y, BoundLowY, BoundHighY);
        }

        void MoveX(Vector3 direction)
        {
            float maxX = MaxSpeed;

            if (Facing == Direction.ForwardFromLeft || Facing == Direction.ForwardFromRight)
            {
                maxX *= 0.5f;
            }

            Velocity.X = MathHelper.Clamp(Velocity.X, -maxX, maxX);
            Acceleration = direction;
            float Deceration = 1.1f;
            Acceleration.Y = -Velocity.Y * Deceration;
        }

        void MoveY(Vector3 direction)
        {
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed * 0.5f, MaxSpeed * 0.33f);
            Acceleration = direction;
            float Deceration = 1.1f;
            Acceleration.X = -Velocity.X * Deceration;
        }

        void TeltChopper()
        {
            float comp = 0.001f;

            switch (Facing)
            {
                case Direction.Left:
                    ChangeXTilt();

                    if (Rotation.Z > ((MoveHorizontal * -Tilt) + (Velocity.X * comp)))
                    {
                        RotationVelocity.Z = -RotateRate * 0.25f;
                    }
                    else
                    {
                        Rotation.Z = (MoveHorizontal * -Tilt) + (Velocity.X * comp);
                    }
                    break;

                case Direction.Right:
                    ChangeXTilt();

                    if (Rotation.Z < (MoveHorizontal * Tilt) - (Velocity.X * comp))
                    {
                        RotationVelocity.Z = RotateRate * 0.25f;
                    }
                    else
                    {
                        Rotation.Z = (MoveHorizontal * Tilt) - (Velocity.X * comp);
                    }
                    break;

                case Direction.ForwardFromRight:
                case Direction.ForwardFromLeft:

                    if (Rotation.X > (MoveHorizontal * Tilt) - (Velocity.X * 0.5f * comp))
                    {
                        RotationVelocity.X = -RotateRate * 0.25f;
                    }
                    else
                    {
                        Rotation.X = (MoveHorizontal * Tilt) - (Velocity.X * 0.5f * comp);
                    }

                    if (Rotation.X < (MoveHorizontal * Tilt) - (Velocity.X * 0.5f * comp))
                    {
                        RotationVelocity.X = RotateRate * 0.25f;
                    }
                    else
                    {
                        Rotation.X = (MoveHorizontal * Tilt) - (Velocity.X * 0.5f * comp);
                    }

                    RotationVelocity.Z = 0;

                    if (Rotation.Z < 0)
                    {
                        RotationVelocity.Z = RotateRate * 0.25f;
                    }
                    else if (Rotation.Z > 0)
                    {
                        RotationVelocity.Z = -RotateRate * 0.25f;
                    }

                    break;
            }

            Rotation.Z = MathHelper.Clamp(Rotation.Z, -MathHelper.PiOver4 * 0.5f, MathHelper.PiOver4 * 0.5f);
        }

        void ChangeXTilt()
        {
            RotationVelocity.X = 0;

            if (Rotation.X < 0)
            {
                RotationVelocity.X = RotateRate * 0.25f;
            }
            else if (Rotation.X > 0)
            {
                RotationVelocity.X = -RotateRate * 0.25f;
            }
        }

        void ChangeFacing()
        {
            FacingChanged = true;

            switch (Facing)
            {
                case Direction.Right:
                    Facing = Direction.ForwardFromRight;
                    break;

                case Direction.ForwardFromRight:
                    Facing = Direction.Left;
                    break;

                case Direction.Left:
                    Facing = Direction.ForwardFromLeft;
                    break;

                case Direction.ForwardFromLeft:
                    Facing = Direction.Right;
                    break;
            }
        }

        void UpdateFacing()
        {
            switch (Facing)
            {
                case Direction.ForwardFromRight:
                    if (Rotation.Y > -MathHelper.PiOver2)
                    {
                        RotationVelocity.Y = -RotateRate;
                    }
                    else
                    {
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.ForwardFromLeft:
                    if (Rotation.Y < -MathHelper.PiOver2)
                    {
                        RotationVelocity.Y = RotateRate;
                    }
                    else
                    {
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.Right:
                    if (Rotation.Y < -0.05)
                    {
                        RotationVelocity.Y = RotateRate;
                    }
                    else
                    {
                        Rotation.Y = 0;
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.Left:
                    if (Rotation.Y > -MathHelper.Pi)
                    {
                        RotationVelocity.Y = -RotateRate;
                    }
                    else
                    {
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;
            }
        }
    }
}
