﻿using Microsoft.Xna.Framework;
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

    public class ThePlayer : Engine.AModel
    {
        enum Direction
        {
            Right,
            ForwardFromRight,
            Left,
            ForwardFromLeft
        };

        Engine.AModel MainBlade;
        Engine.AModel Rotor;
        Shot[] Shots = new Shot[5];

        T FireTimer;
        T TurnTimer;

        KeyboardState KeyState;
        KeyboardState KeyStateOld;

        float AccelerationAmount = 220;
        float MaxSpeed = 350;
        float Tilt = MathHelper.PiOver4 / 10f;
        float FireRate = 0.1f;
        float TurnRate = 1.1f;
        float RotateRate = MathHelper.PiOver2;
        float MoveHorizontal;
        float BoundLowY = -219.5f;
        float BoundHighY = 364.25f;
        float BoundRightX = 145.5f;
        float BoundLeftX = -6600 * 2;
        int ShotLimit = 5;
        bool FacingChanged;
        bool Coasting;

        Direction Facing;

        public ThePlayer(Game game) : base(game)
        {
            MainBlade = new Engine.AModel(game);
            Rotor = new Engine.AModel(game);
            FireTimer = new T(game, FireRate);
            TurnTimer = new T(game, TurnRate);

            for(int i = 0; i < ShotLimit; i++)
            {
                Shots[i] = new Shot(game);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Facing = Direction.Left;

        }

        public void LoadContent()
        {
            MainBlade.LoadModel(Game.Content.Load<XnaModel>("Models/CLPlayerMainBlade"), null);
            Rotor.LoadModel(Game.Content.Load<XnaModel>("Models/CLPlayerRotor"), null);
            LoadModel(Game.Content.Load<XnaModel>("Models/CLPlayerChopper"), null);

            XnaModel shotM = Game.Content.Load<XnaModel>("Models/cube");

            for (int i = 0; i < ShotLimit; i++)
            {
                Shots[i].LoadModel(shotM);
            }
        }

        public override void BeginRun()
        {
            base.BeginRun();

            AddChild(MainBlade, true, true);
            AddChild(Rotor, true, true);

            Children[0].ReletivePosition = new Vector3(0, 12f, 0);
            Children[1].ReletivePosition = new Vector3(25, 8f, 0);
            Children[0].RotationVelocity = new Vector3(0, 10, 0);
            Children[1].RotationVelocity = new Vector3(0, 0, 16);

            for (int i = 0; i < ShotLimit; i++)
            {
                Shots[i].Active = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            S.Camera.Position.X = Position.X;

            GetInput();
            TeltChopper();

            if (Position.X < BoundLeftX || Position.X > BoundRightX || Position.Y > BoundHighY || Position.Y < BoundLowY)
            {
                StopMovementX();
                StopMovementY();
            }

            if (FacingChanged)
                UpdateFacing();

            if (Coasting)
                WindResistance();

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

            //+		Position	62.65797  -126.4613  0	Microsoft.Xna.Framework.Vector3
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

            KeyStateOld = KeyState;
        }

        void FireShot()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!Shots[i].Active)
                {
                    float rot = Rotation.Z - MathHelper.Pi;

                    if (rot < 0)
                        rot += MathHelper.TwoPi;


                    Shots[i].Spawn(Position + new Vector3(-20, -5, 0), SetVelocityFromAngle(rot, 200f - Velocity.X));

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
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
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
            float comp = 0.002f;

            switch (Facing)
            {
                case Direction.Right:
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

                case Direction.Left:
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
                    if (Rotation.X > (MoveHorizontal * -Tilt) + (Velocity.X * comp))
                    {
                        RotationVelocity.X = -RotateRate * 0.25f;
                    }
                    else
                    {
                        Rotation.X = (MoveHorizontal * -Tilt) + (Velocity.X * comp);
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
                    if (Rotation.Y > MathHelper.PiOver2)
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
                    if (Rotation.Y < MathHelper.PiOver2)
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
                    if (Rotation.Y < Math.PI)
                    {
                        RotationVelocity.Y = RotateRate;
                    }
                    else
                    {
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;

                case Direction.Left:
                    if (Rotation.Y > 0.05f)
                    {
                        RotationVelocity.Y = -RotateRate;
                    }
                    else
                    {
                        Rotation.Y = 0;
                        RotationVelocity.Y = 0;
                        FacingChanged = false;
                    }
                    break;
            }
        }
    }
}
