using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Engine;

namespace MGChoplifter.Entities
{
    using Sys = Services;
    using Time = Timer;
    using PO = PositionedObject;
    using Mod = AModel;

    public class HouseControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        Mod[] Houses = new Mod[4];
        Mod[] OpenHouses = new Mod[4];

        List<Person> People = new List<Person>();

        ThePlayer PlayerRef;

        XnaModel PersonMan;
        XnaModel PersonArm;
        XnaModel PersonLeg;

        float Height = -230;
        float StartX = -6000;
        float DistanceBetween = 600;

        public HouseControl(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i] = new Mod(game);
                OpenHouses[i] = new Mod(game);
            }

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            Sys.AddBeginable(this);
            Sys.AddLoadable(this);
        }

        public void LoadContent()
        {
            XnaModel houseModel = Game.Content.Load<XnaModel>("Models/CLHouse");
            XnaModel openModel = Game.Content.Load<XnaModel>("Models/CLHouseOpen");

            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i].SetModel(houseModel);
                OpenHouses[i].SetModel(openModel);
            }

            PersonMan = Game.Content.Load<XnaModel>("Models/CLPersonMan");
            PersonArm = Game.Content.Load<XnaModel>("Models/CLPersonArm");
            PersonLeg = Game.Content.Load<XnaModel>("Models/CLPersonLeg");

            PlayerRef.PersonMan = PersonMan;
            PlayerRef.PersonArm = PersonArm;
            PlayerRef.PersonLeg = PersonLeg;
        }

        public void BeginRun()
        {
            for (int i = 0; i < Houses.Length; i++)
            {
                Houses[i].Position = new Vector3(StartX - (i * DistanceBetween), Height, -100);
                Houses[i].Radius = 32;
                OpenHouses[i].Position = Houses[i].Position;
                OpenHouses[i].Active = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Mod house in Houses)
            {
                if (house.Active)
                {
                    foreach (Shot shot in PlayerRef.Shots)
                    {
                        if (shot.Active)
                        {
                            if (house.CirclesIntersect(shot))
                            {
                                HouseHit(house.Position);
                                house.Active = false;
                                shot.Active = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        void HouseHit(Vector3 position)
        {
            foreach (Mod house in OpenHouses)
            {
                if (house.Position == position)
                {
                    house.Active = true;
                    break;
                }
            }

            SpawnPeople(position);
        }

        void SpawnPeople(Vector3 position)
        {
            for (int p = 0; p < 4; p++)
            {
                bool spawnNew = true;

                foreach (Person man in People)
                {
                    if (!man.Active)
                    {
                        spawnNew = false;
                        man.Spawn(position, false);
                        break;
                    }
                }

                if (spawnNew)
                {
                    People.Add(new Person(Game, PlayerRef));
                    People.Last().SetModel(PersonMan);

                    for (int i = 0; i < 2; i++)
                    {
                        People.Last().Arms[i].SetModel(PersonArm);
                        People.Last().Legs[i].SetModel(PersonLeg);
                    }

                    People.Last().Spawn(position, false);
                }
            }
        }
    }
}
