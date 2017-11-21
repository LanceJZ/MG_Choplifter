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
    using PO = PositionedObject;

    public class EnemyControl : GameComponent, IBeginable, IUpdateableComponent
    {
        public ThePlayer PlayerRef;
        List<Tank> Tanks;



        public EnemyControl(Game game, ThePlayer player) : base(game)
        {
            PlayerRef = player;
            Tanks = new List<Tank>();

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            SpawnTanks(6);
            Services.AddBeginable(this);
        }

        public void BeginRun()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CheckOtherTanks();
            CheckTankHit();
        }

        void SpawnTanks(int amount)
        {
            for (int a = 0; a < amount; a++)
            {
                bool spawnNew = true;
                int spawnNumber = 0;

                for (int i = 0; i < Tanks.Count; i++)
                {
                    if (!Tanks[i].Active)
                    {
                        spawnNew = false;
                        spawnNumber = i;
                        break;
                    }
                }

                if (spawnNew)
                {
                    spawnNumber = Tanks.Count;
                    Tanks.Add(new Tank(Game, PlayerRef));
                }

                Tanks[spawnNumber].Spawn();
            }
        }

        void CheckOtherTanks()
        {
            if (Tanks.Count < 2)
                return;

            foreach (Tank tank in Tanks)
            {
                tank.NotBumped();
            }

            foreach (Tank tanka in Tanks)
            {
                foreach (Tank tankb in Tanks)
                {
                    if (tanka != tankb)
                    {
                        if (tanka.Active && tankb.Active)
                        {
                            if (tanka.CirclesIntersect(tankb))
                            {
                                if (tanka.Position.X > tankb.Position.X)
                                {
                                    tanka.BumpedL();
                                    tankb.BumpedR();
                                }
                                else
                                {
                                    tanka.BumpedR();
                                    tankb.BumpedL();
                                }
                            }
                        }
                    }
                }
            }
        }

        void CheckTankHit()
        {
            foreach (Tank tank in Tanks)
            {
                if (tank.Active)
                {
                    foreach (Shot shot in PlayerRef.Shots)
                    {
                        if (shot.Active)
                        {
                            if (shot.CirclesIntersect(tank))
                            {
                                shot.Active = false;
                                tank.Active = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
