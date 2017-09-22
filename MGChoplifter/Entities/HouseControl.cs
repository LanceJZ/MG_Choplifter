using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class HouseControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent, Engine.ILoadContent
    {
        Engine.AModel[] Houses = new Engine.AModel[4];
        Engine.AModel[] OpenHouses = new Engine.AModel[4];

        List<Person> People = new List<Person>();

        //Person TestPerson;

        ThePlayer PlayerRef;

        XnaModel PersonBody;
        XnaModel PersonArm;
        XnaModel PersonLeg;

        float Height = -230;
        float StartX = -6000;
        float DistanceBetween = 600;

        public HouseControl(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;

            //TestPerson = new Person(game, player);

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
            S.AddLoadable(this);

            //TestPerson.Position = new Vector3(-200, -225, 0);
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

            PersonBody = Game.Content.Load<XnaModel>("Models/CLPersonMan");
            PersonArm = Game.Content.Load<XnaModel>("Models/CLPersonArm");
            PersonLeg = Game.Content.Load<XnaModel>("Models/CLPersonLeg");

            //SpawnPeople(TestPerson.Position);
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

            //TestPerson.SetModel(PersonBody);

            //for (int i = 0; i < 2; i++)
            //{
            //    TestPerson.Arms[i].SetModel(PersonArm);
            //    TestPerson.Legs[i].SetModel(PersonLeg);
            //}

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Engine.AModel house in Houses)
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
            foreach (Engine.AModel house in OpenHouses)
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
                        man.Spawn(position);
                        break;
                    }
                }

                if (spawnNew)
                {
                    People.Add(new Person(Game, PlayerRef));
                    People.Last().SetModel(PersonBody);

                    for (int i = 0; i < 2; i++) //Figure out why arms and legs are invisable.
                    {
                        People.Last().Arms[i].SetModel(PersonArm);
                        People.Last().Legs[i].SetModel(PersonLeg);
                    }

                    People.Last().Spawn(position);
                }
            }
        }
    }
}
