using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MGChoplifter.Entities
{
    using S = Engine.Services;
    using T = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class EnemyControl : GameComponent, Engine.IBeginable, Engine.IUpdateableComponent
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
            Tank.Position.X = -1000;
            Tank.PlayerRef = PlayerRef;

            S.AddBeginable(this);
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
