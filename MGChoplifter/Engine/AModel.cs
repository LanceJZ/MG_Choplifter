#region Using
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
#endregion

namespace Engine
{
    public class AModel : PositionedObject, IDrawComponent
    {
        public XnaModel xnaModel { get; private set; }
        Texture2D xnaTexture;
        private Matrix[] modelTransforms;
        private Matrix baseWorld;

        public AModel (Game game) : base(game)
        {

        }

        public AModel(Game game, XnaModel model, Texture2D texture) : base(game)
        {
            LoadModel(model, texture);
        }

        public override void Initialize()
        {
            base.Initialize();
            Enabled = true;
            Services.AddDrawableComponent(this);

            Services.GraphicsDM.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Services.GraphicsDM.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            Services.GraphicsDM.GraphicsDevice.BlendState = BlendState.Opaque;
            Services.GraphicsDM.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculate the base transformation by combining
            // translation, rotation, and scaling
            baseWorld = Matrix.Identity;

            baseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(ReletivePosition)
                * Matrix.CreateFromYawPitchRoll(ReletiveRotation.Y, ReletiveRotation.X, ReletiveRotation.Z)
                * Matrix.CreateTranslation(Position);
        }

        public void Draw(GameTime gametime)
        {

            if (xnaModel == null)
                return;

            foreach (ModelMesh mesh in xnaModel.Meshes)
            {
                Matrix localWorld = modelTransforms[mesh.ParentBone.Index]
                   * baseWorld;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)meshPart.Effect;
                    effect.TextureEnabled = true;

                    if (xnaTexture != null)
                        effect.Texture = xnaTexture;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = localWorld;
                    Services.Camera.Draw(effect);
                }

                mesh.Draw();
            }

        }

        public void LoadModel(XnaModel model, Texture2D texture)
        {
            xnaModel = model;
            xnaTexture = texture;
            modelTransforms = new Matrix[xnaModel.Bones.Count];
            xnaModel.CopyAbsoluteBoneTransformsTo(modelTransforms);
        }
    }
}
