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
    using Sys = Services;
    using Time = Timer;
    using PO = PositionedObject;

    public class EnemyControl : GameComponent, IBeginable, IUpdateableComponent
    {
        public ThePlayer PlayerRef;
        Tank Tank;

        public EnemyControl(Game game, ThePlayer player) : base(game)
        {
            Tank = new Tank(game, player);
            PlayerRef = player;

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            Tank.Position.Y = -280;
            Tank.Position.X = -5000;
            Tank.PlayerRef = PlayerRef;

            Sys.AddBeginable(this);
        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}
