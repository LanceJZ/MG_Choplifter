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

    public class HouseControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent
    {
        Engine.AModel[] Houses = new Engine.AModel[4];
        Engine.AModel[] OpenHouses = new Engine.AModel[4];

        float Height = -230;
        float StartX = -11000;
        float DistanceBetween = 600;

        public HouseControl(Game game) : base(game)
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i] = new Engine.AModel(game);
                OpenHouses[i] = new Engine.AModel(game);
            }

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            S.AddBeginable(this);
        }

        public void LoadContent()
        {
            XnaModel houseModel = Game.Content.Load<XnaModel>("Models/CLHouse");
            XnaModel openModel = Game.Content.Load<XnaModel>("Models/CLHouseOpen");

            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i].LoadModel(houseModel, null);
                OpenHouses[i].LoadModel(openModel, null);
            }
        }

        public void BeginRun()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i].Position = new Vector3(StartX - (i * DistanceBetween), Height, -100);
                OpenHouses[i].Position = Houses[i].Position;
                OpenHouses[i].Active = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < Houses.Length; i++)
            {

            }
        }
    }
}
