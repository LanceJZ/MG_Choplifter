using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class Background : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent
    {
        MountianControl Mountians;
        StarControl Stars;
        Engine.AModel Base;

        Engine.Plane[] Grass = new Engine.Plane[30];
        float[] GrassX;
        float DistanceBetween = 85;

        public Background(Game game) : base(game)
        {
            GrassX = new float[Grass.Length];

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i] = new Engine.Plane(game);
            }

            Mountians = new MountianControl(game);
            Stars = new StarControl(game);
            Base = new Engine.AModel(game);

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            S.AddBeginable(this);
        }

        public void LoadContent()
        {
            Base.LoadModel(Game.Content.Load<XnaModel>("Models/CLBaseV2"), null);

            Texture2D grass = Game.Content.Load<Texture2D>("Textures/Grass");

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i].Create(grass);
            }

        }

        public void BeginRun()
        {
            Base.Position = new Vector3(100, -234, -50);

            float startX = 10 + -DistanceBetween * (Grass.Length * 0.25f);
            float startY = -267;

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i].Position.Z = -200;
            }

            for (int i = 0; i < (int)(Grass.Length * 0.5f); i++)
            {
                Grass[i].Position.X = startX + (i * DistanceBetween);
                Grass[i].Position.Y = startY;
            }

            for (int i = 0; i < (int)(Grass.Length * 0.5f); i++)
            {
                Grass[i + (int)(Grass.Length * 0.5f)].Position.X = startX + (i * DistanceBetween);
                Grass[i + (int)(Grass.Length * 0.5f)].Position.Y = startY - DistanceBetween;
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                GrassX[i] = Grass[i].Position.X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Grass.Length; i++)
            {
                for (int n = 0; n < 2; n++)
                {
                    Grass[i].Position.X = GrassX[i] + S.Camera.Position.X;
                }
            }

        }
    }
}
