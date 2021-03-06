﻿using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using Engine;

namespace MGChoplifter.Entities
{
    public class TankTurret : AModel
    {
        public ThePlayer PlayerRef;
        AModel Barral;
        Shot TankShot;
        Timer ShotTimer;

        public TankTurret(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

            Barral = new AModel(game);
            ShotTimer = new Timer(game, 4.1f);
        }

        public override void Initialize()
        {
            base.Initialize();

            Barral.AddAsChild(this, true, false);
            Barral.Position.X = 8;
            LoadContent();
        }

        public override void LoadContent()
        {
            SetModel(Game.Content.Load<XnaModel>("Models/CLTankTurret"));
            Barral.SetModel(Game.Content.Load<XnaModel>("Models/CLTankBarral"));
            TankShot = new Shot(Game, Load("cube"));
            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            TankShot.Active = false;
            TankShot.Acceleration.Y = -50;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 target = new Vector2(PlayerRef.Position.X, 100);
            Vector2 pos = new Vector2(WorldPosition.X, WorldPosition.Y);
            Rotation.Y = AngleToTurret(pos, target);

            Barral.Rotation.Z = MathHelper.Clamp(AngleToBarral(pos, target), 0, MathHelper.PiOver4);

            if (ShotTimer.Expired && Active)
            {
                ShotTimer.Reset();
                FireShot();
            }
        }

        void FireShot()
        {
            Vector2 pos = new Vector2(Barral.WorldPosition.X, Barral.WorldPosition.Y);
            Vector2 target = new Vector2(PlayerRef.Position.X, 100);
            Vector2 pvel = new Vector2(ParentPO.Velocity.X, ParentPO.Velocity.Y);

            TankShot.Spawn(WorldPosition, SetVelocity(AngleFromVectors(pos, target), 100) + pvel, 3.5f);
        }

        float AngleToBarral(Vector2 pos, Vector2 target)
        {
            Vector2 diference = Vector2.Zero;

            if (target.X > pos.X)
                diference = target - pos;
            else
                diference = pos - target;

            return Angle(new Vector2(1, 0), diference);
        }

        float AngleToTurret(Vector2 pos, Vector2 target)
        {
            Vector2 diference = target - pos;

            float sign = (target.Y < pos.Y) ? -1.0f : 1.0f;
            return Angle(new Vector2(1, 0), diference) * sign;
        }

        float Angle(Vector2 direction, Vector2 differnce)
        {
            float angle = AngleFromVectors(direction, differnce);

            if (angle < 0)
                angle *= -1;

            return MathHelper.Clamp(angle, 0, MathHelper.Pi);
        }
    }
}
