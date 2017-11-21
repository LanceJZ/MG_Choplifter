using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;

namespace MGChoplifter.Entities
{
    using PO = PositionedObject;
    using Mod = AModel;

    public class Background : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        ThePlayer PlayerRef;
        StarControl Stars;
        Mod Base;

        Engine.Plane[] Grass = new Engine.Plane[51];
        Mod[] Blockades = new Mod[4];
        Mod[] Mountians = new Mod[8];

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
                Mountians[i] = new Mod(game);
            }

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i] = new Engine.Plane(game);
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i] = new Mod(game);
            }

            Stars = new StarControl(game);
            Base = new Mod(game);

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);
            Services.AddLoadable(this);

            base.Initialize();
        }

        public void LoadContent()
        {
            Base.LoadModel("CLBaseV2");
            XnaModel block = Blockades[0].Load("CLBarakade");
            XnaModel mount = Mountians[0].Load("Mountain");
            Texture2D grass = Grass[0].Load("Grass");

            for (int i = 0; i < Grass.Length; i++)
            {
                Grass[i].Create(grass);
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].SetModel(block, null);
            }

            for (int i = 0; i < Mountians.Length; i ++)
            {
                Mountians[i].SetModel(mount, null);
            }
        }

        public void BeginRun()
        {
            Base.Position = new Vector3(100, -234, -50);

            float spaceBetweenBlocks = -8;
            float spaceBetweenMounts = -550f;
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
                    if (Grass[i].Position.X - spaceBetweenGrass > Services.Camera.Position.X + GrassEdge)
                    {
                        Grass[i].Position.X -= 1200 + spaceBetweenGrass * 2;
                    }
                }

                if (PlayerRef.Velocity.X > 0)
                {
                    if (Grass[i].Position.X + spaceBetweenGrass < Services.Camera.Position.X - GrassEdge)
                    {
                        Grass[i].Position.X += 1200 + spaceBetweenGrass * 2;
                    }
                }
            }

            for (int i = 0; i < Blockades.Length; i++)
            {
                Blockades[i].Position.X = BlocksX[i] - ((Services.Camera.Position.X - BlocksX[i]) * (0.2f * i));
            }

            for (int i = 0; i < Mountians.Length; i++)
            {
                Mountians[i].Position.X = MountianX[i] + (Services.Camera.Position.X * 0.35f);
            }

            base.Update(gameTime);
        }
    }
}
