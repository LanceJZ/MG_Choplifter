﻿#region Using
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Engine
{
	public class PositionedObject : GameComponent, IBeginable
	{
		#region Fields
		private float m_ElapsedGameTime;
		// Doing these as fields is almost twice as fast as if they were properties.
		// Also, sense XYZ are fields they do not get data binned as a property.
		public List<PositionedObject> Children;
		public Vector3 Position = Vector3.Zero;
		public Vector3 Acceleration = Vector3.Zero;
		public Vector3 Velocity = Vector3.Zero;
		public Vector3 ReletivePosition = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 ReletiveRotation = Vector3.Zero;
        public Vector3 RotationVelocity = Vector3.Zero;
        public Vector3 RotationAcceleration = Vector3.Zero;
		public Rectangle AABB = Rectangle.Empty; // The axis-aligned bounding box.
		//short[] indexData; // The index array used to render the AABB.
		//VertexPositionColor[] aabbVertices; // The AABB vertex array (used for rendering).
		float m_ScalePercent = 1;
		float m_Radius = 0;
		bool m_Hit = false;
		bool m_ExplosionActive = false;
		bool m_Pause = false;
		bool m_Moveable = true;
		bool m_Active = true;
		bool m_ActiveDependent;
		bool m_DirectConnection;
		bool m_Parent;
		bool m_Child;
		bool m_Debug;
		#endregion
		#region Properties
		public float ElapsedGameTime { get => m_ElapsedGameTime; }
        /// <summary>
        /// Scale by percent of original. If base of sprite, used to enlarge sprite.
        /// </summary>
		public float Scale	{ get => m_ScalePercent; set => m_ScalePercent = value; }
        /// <summary>
        /// Used for circle collusion. Sets radius of circle.
        /// </summary>
		public float Radius { get => m_Radius; set => m_Radius = value; }
        /// <summary>
        /// Enabled means this class is a parent, and has at least one child.
        /// </summary>
		public bool Parent { get => m_Parent; set => m_Parent = value; }
        /// <summary>
        /// Enabled means this class is a child to a parent.
        /// </summary>
		public bool Child { get => m_Child; set => m_Child = value; }
        /// <summary>
        /// Enabled tells class hit by another class.
        /// </summary>
		public bool Hit { get => m_Hit; set => m_Hit = value; }
        /// <summary>
        /// Enabled tells class an explosion is active.
        /// </summary>
		public bool ExplosionActive { get => m_ExplosionActive; set => m_ExplosionActive = value; }
        /// <summary>
        /// Enabled pauses class update.
        /// </summary>
		public bool Pause { get => m_Pause; set => m_Pause = value; }
        /// <summary>
        /// Enabled will move using velocity and acceleration.
        /// </summary>
		public bool Moveable { get => m_Moveable; set => m_Moveable = value; }
        /// <summary>
        /// Enabled causes the class to update. If base of Sprite, enables sprite to be drawn.
        /// </summary>
		public bool Active { get => m_Active; set => m_Active = value; }
        /// <summary>
        /// Enabled the active bool will mirror that of the parent.
        /// </summary>
		public bool ActiveDependent { get => m_ActiveDependent; set => m_ActiveDependent = value; }
        /// <summary>
        /// Enabled the position and rotation will always be the same as the parent.
        /// </summary>
		public bool DirectConnection { get => m_DirectConnection; set => m_DirectConnection = value; }
		/// <summary>
		/// Gets or sets the GameModel's AABB
		/// </summary>
		public bool Debug { set => m_Debug = value; }
		#endregion
		#region Constructor
		/// <summary>
		/// This is the constructor that gets the Positioned Object ready for use and adds it to the Drawable Components list.
		/// </summary>
		/// <param name="game">The game class</param>
		public PositionedObject(Game game) : base(game)
		{
			game.Components.Add(this);
			Children = new List<PositionedObject>();
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// Allows the game component to be updated.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (Moveable && Active)
			{
				base.Update(gameTime);

				m_ElapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
				Velocity += Acceleration * m_ElapsedGameTime;
				Position += Velocity * m_ElapsedGameTime;
				AABB.X = (int)(Position.X);
				AABB.Y = (int)(Position.Y);
				RotationVelocity += RotationAcceleration * m_ElapsedGameTime;
				Rotation += RotationVelocity * m_ElapsedGameTime;

				//if (Rotation.X > MathHelper.TwoPi)
				//	Rotation.X = -MathHelper.TwoPi;

				//if (Rotation.Y < -MathHelper.TwoPi)
				//	Rotation.Y = MathHelper.TwoPi;

    //            if (Rotation.Z > MathHelper.TwoPi)
    //                Rotation.Z = -MathHelper.TwoPi;

    //            if (Rotation.X < -MathHelper.TwoPi)
    //                Rotation.X = MathHelper.TwoPi;

    //            if (Rotation.Y > MathHelper.TwoPi)
    //                Rotation.Y = -MathHelper.TwoPi;

    //            if (Rotation.Z < -MathHelper.TwoPi)
    //                Rotation.Z = MathHelper.TwoPi;
            }

            if (m_Parent)
			{
				foreach (PositionedObject child in Children)
				{
					if (Active)
					{
						if (child.DirectConnection)
						{
							child.Position = Position;
                            child.ReletiveRotation = Rotation;
							child.Scale = Scale;
						}
						else
						{
                            child.Position = Vector3.Transform(child.ReletivePosition,
                                Matrix.CreateRotationX(Rotation.X));
                            child.Position = Vector3.Transform(child.ReletivePosition,
                                Matrix.CreateRotationY(Rotation.Y));
                            child.Position = Vector3.Transform(child.ReletivePosition,
                                Matrix.CreateRotationZ(Rotation.Z));
                            child.Position += Position;
                            child.Rotation = Rotation + child.ReletiveRotation;
                        }
					}

					if (child.ActiveDependent)
						child.Active = Active;
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			AABB = new Rectangle();
			Services.AddBeginable(this);
		}

		public virtual void BeginRun()
		{

		}
        /// <summary>
        /// Add PO class or base PO class from Sprite as child of this class.
        /// </summary>
        /// <param name="child">The child to this class.</param>
        /// <param name="activeDependent">Bind Active to child.</param>
        /// <param name="directConnection">Bind Position and Rotation to child.</param>
		public virtual void AddChild(PositionedObject child, bool activeDependent, bool directConnection)
		{
			Children.Add(child);
			Children[Children.Count - 1].ActiveDependent = activeDependent;
			Children[Children.Count - 1].DirectConnection = directConnection;
            Children[Children.Count - 1].Child = true;
            m_Parent = true;
		}
        /// <summary>
        /// AABB collusion detection.
        /// </summary>
        /// <param name="heightWidth">Size of AABB height and width.</param>
		public void SetAABB(Vector2 heightWidth)
		{
			AABB = new Rectangle((int)Position.X, (int)Position.Y, (int)(heightWidth.X * m_ScalePercent),
				(int)(heightWidth.Y * m_ScalePercent));
		}

		public void Remove()
		{
			Game.Components.Remove(this);
		}
        /// <summary>
        /// Circle collusion detection. Target circle will be compared to this class's.
        /// Will return true of they intersect.
        /// </summary>
        /// <param name="Target">Position of target.</param>
        /// <param name="TargetRadius">Radius of target.</param>
        /// <returns></returns>
		public bool CirclesIntersect(Vector2 Target, float TargetRadius)
		{
			float distanceX = Target.X - Position.X;
			float distanceY = Target.Y - Position.Y;
			float radius = Radius + TargetRadius;

			if ((distanceX * distanceX) + (distanceY * distanceY) < radius * radius)
				return true;

			return false;
		}
        /// <summary>
        /// Returns a Vector2 direction of travel from angle and magnitude.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="magnitude"></param>
        /// <returns>Vector2</returns>
        public Vector2 SetVelocity(float angle, float magnitude)
        {
            Vector2 Vector = new Vector2(0);
            Vector.Y = (float)(Math.Sin(angle) * magnitude);
            Vector.X = (float)(Math.Cos(angle) * magnitude);
            return Vector;
        }
        /// <summary>
        /// Returns a float of the angle in radians derived from two Vector2 passed into it, using only the X and Y.
        /// </summary>
        /// <param name="origin">Vector2 of origin</param>
        /// <param name="target">Vector2 of target</param>
        /// <returns>Float</returns>
        public float AngleFromVectors(Vector2 origin, Vector2 target)
        {
            return (float)(Math.Atan2(target.Y - origin.Y, target.X - origin.X));
        }

        public float RandomRadian()
        {
            return Services.RandomMinMax(0, (float)Math.PI * 2);
        }

        public Vector2 SetRandomVelocity(float speed)
        {
            float ang = RandomRadian();
            float amt = Services.RandomMinMax(speed * 0.15f, speed);
            return SetVelocityFromAngle(ang, amt);
        }

        public Vector2 SetRandomVelocity(float speed, float radianDirection)
        {
            float amt = Services.RandomMinMax(speed * 0.15f, speed);
            return SetVelocityFromAngle(radianDirection, amt);
        }

        public Vector2 SetVelocityFromAngle(float rotation, float magnitude)
        {
            return new Vector2((float)Math.Cos(rotation) * magnitude, (float)Math.Sin(rotation) * magnitude);
        }

        public Vector2 SetVelocityFromAngle(float magnitude)
        {
            float ang = RandomRadian();
            return new Vector2((float)Math.Cos(ang) * magnitude, (float)Math.Sin(ang) * magnitude);
        }

        public Vector2 SetRandomEdge()
        {
            return new Vector2(Services.WindowWidth * 0.5f,
                Services.RandomMinMax(-Services.WindowHeight * 0.45f, Services.WindowHeight * 0.45f));
        }

        public float AimAtTarget(Vector2 origin, Vector2 target, float facingAngle, float magnitude)
        {
            float turnVelocity = 0;
            float targetAngle = AngleFromVectors(origin, target);
            float targetLessFacing = targetAngle - facingAngle;
            float facingLessTarget = facingAngle - targetAngle;

            if (Math.Abs(targetLessFacing) > Math.PI)
            {
                if (facingAngle > targetAngle)
                {
                    facingLessTarget = ((MathHelper.TwoPi - facingAngle) + targetAngle) * -1;
                }
                else
                {
                    facingLessTarget = (MathHelper.TwoPi - targetAngle) + facingAngle;
                }
            }

            if (facingLessTarget > 0)
            {
                turnVelocity = -magnitude;
            }
            else
            {
                turnVelocity = magnitude;
            }

            return turnVelocity;
        }

        public void CheckWindowBorders()
        {
            if (Position.X > Services.WindowWidth)
                Position.X = 0;

            if (Position.X < 0)
                Position.X = Services.WindowWidth;

            if (Position.Y > Services.WindowHeight)
                Position.Y = 0;

            if (Position.Y < 0)
                Position.Y = Services.WindowHeight;
        }

        public void CheckWindowSideBorders(int width)
        {
            if (Position.X + width > Services.WindowWidth)
                Position.X = 0;

            if (Position.X < 0)
                Position.X = Services.WindowWidth - width;
        }

        #endregion
	}
}
