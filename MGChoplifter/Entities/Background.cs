﻿using Microsoft.Xna.Framework;
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

    public class Background : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent, Engine.ILoadContent
    {
        ThePlayer PlayerRef;
        StarControl Stars;
        Engine.AModel Base;

        Engine.Plane[] Grass = new Engine.Plane[51];
        Engine.AModel[] Blockades = new Engine.AModel[4];
        Engine.AModel[] Mountians = new Engine.AModel[8];

        float spaceBetweenGrass = 85;
        float GrassEdge = 600;
        float[] MountianX;
        float[] GrassX;
        float[] BlocksX;

        public Background(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

            GrassX = new float[Grass.Length];
            BlocksX = new float[Blockades.Length];
            MountianX = new float[Mountians.Length];

            for (int i = 0; i < Mountians.Length; i++)
            {
                Mountians[i] = new Engine.AModel(game);
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i] = new Engine.Plane(game);
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i] = new Engine.AModel(game);
            }

            Stars = new StarControl(game);
            Base = new Engine.AModel(game);

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            S.AddBeginable(this);
            S.AddLoadable(this);
        }

        public void LoadContent()
        {
            Base.LoadModel(Game.Content.Load<XnaModel>("Models/CLBaseV2"), null);
            XnaModel block = Game.Content.Load<XnaModel>("Models/CLBarakade");
            XnaModel mount = Game.Content.Load<XnaModel>("Models/Mountain");
            Texture2D grass = Game.Content.Load<Texture2D>("Textures/Grass");

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i].Create(grass);
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].LoadModel(block, null);
            }

            for (int i = 0; i < Mountians.Length; i ++)
            {
                Mountians[i].LoadModel(mount, null);
            }
        }

        public void BeginRun()
        {
            Base.Position = new Vector3(100, -234, -50);

            float spaceBetweenBlocks = -8;
            float spaceBetweenMounts = -830f;
            float startGrassX = 10 + -spaceBetweenGrass * (int)(Grass.Length / 6);
            float startGrassY = -267;
            float startBlockX = -600;
            float startBlockY = -250;
            float startMountsX = 443f;
            float startMountsY = -225f;

            for (int i = 0; i < Mountians.Length; i++)
            {
                Mountians[i].Position = new Vector3((i * spaceBetweenMounts) + startMountsX, startMountsY, -250);
                MountianX[i] = Mountians[i].Position.X;
            }

            for (int i = 0; i < (int)(Grass.Length / 3); i++)
            {
                Grass[i].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[i].Position.Y = startGrassY;

                int leveltwo = i + (int)(Grass.Length / 3);
                int levelthree = i + (int)(Grass.Length / 3) * 2;

                Grass[leveltwo].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[leveltwo].Position.Y = startGrassY - spaceBetweenGrass * 0.75f;
                Grass[levelthree].Position.X = startGrassX + (i * spaceBetweenGrass);
                Grass[levelthree].Position.Y = startGrassY - (spaceBetweenGrass * 1.7f);
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                GrassX[i] = Grass[i].Position.X;
                Grass[i].Position.Z = -200;
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].Position = new Vector3(startBlockX, startBlockY +
                    (i * (spaceBetweenBlocks + (i * spaceBetweenBlocks * 0.95f))), -150);
                BlocksX[i] = Blockades[i].Position.X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            for (int i = 0; i < Grass.Length / 3; i++)
            {
                int leveltwo = i + (int)(Grass.Length / 3);
                int levelthree = i + (int)(Grass.Length / 3) * 2;

                Grass[leveltwo].Velocity.X = -PlayerRef.Velocity.X * 0.25f;

                Grass[levelthree].Velocity.X = -PlayerRef.Velocity.X * 0.5f;
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                if (PlayerRef.Velocity.X < 0)
                {
                    if (Grass[i].Position.X - spaceBetweenGrass > S.Camera.Position.X + GrassEdge)
                    {
                        Grass[i].Position.X -= 1200 + spaceBetweenGrass * 2;
                    }
                }

                if (PlayerRef.Velocity.X > 0)
                {
                    if (Grass[i].Position.X + spaceBetweenGrass < S.Camera.Position.X - GrassEdge)
                    {
                        Grass[i].Position.X += 1200 + spaceBetweenGrass * 2;
                    }
                }
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].Position.X = BlocksX[i] - ((S.Camera.Position.X - BlocksX[i]) * (0.2f * i));
            }

            for (int i = 0; i < Mountians.Length; i++)
            {
                Mountians[i].Position.X = MountianX[i] + (S.Camera.Position.X * 0.5f);
            }
        }
    }
}
