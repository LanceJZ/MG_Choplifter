using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class MountianControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent
    {
        float SpaceBetween = -830f;
        float StartX = 443f;
        float Height = -225f;
        float Depth = -150;

        PO[] MountianPOs = new PO[8];
        Vector3[] MountianStartPos;

        public MountianControl(Game game) : base(game)
        {
            MountianStartPos = new Vector3[MountianPOs.Length];

            for (int i = 0; i < MountianPOs.Length; i++)
            {
                MountianPOs[i] = new Engine.AModel(game, game.Content.Load<XnaModel>("Models/Mountain"), null);
                MountianPOs[i].Position = new Vector3((i * SpaceBetween) + StartX, Height, Depth);
                MountianStartPos[i] = MountianPOs[i].Position;
            }

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            S.AddBeginable(this);
        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < MountianPOs.Length; i++)
            {
                MountianPOs[i].Position = new Vector3(MountianStartPos[i].X + (S.Camera.Position.X * 0.5f), Height, Depth);
            }
        }
    }
}
