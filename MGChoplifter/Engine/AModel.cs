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
        Texture2D XNATexture;
        private Matrix[] ModelTransforms;
        private Matrix BaseWorld;

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
            BaseWorld = Matrix.Identity;

            BaseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(ReletivePosition)
                * Matrix.CreateFromYawPitchRoll(ReletiveRotation.Y, ReletiveRotation.X, ReletiveRotation.Z)
                * Matrix.CreateTranslation(Position);
        }

        public void Draw(GameTime gametime)
        {
            if (Active)
            {
                if (xnaModel == null)
                    return;

                foreach (ModelMesh mesh in xnaModel.Meshes)
                {
                    //Matrix localWorld = modelTransforms[mesh.ParentBone.Index]
                    //   * baseWorld;

                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        BasicEffect effect = (BasicEffect)meshPart.Effect;
                        //effect.TextureEnabled = true;

                        if (XNATexture != null)
                            effect.Texture = XNATexture;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = BaseWorld;
                        Services.Camera.Draw(effect);
                    }

                    mesh.Draw();
                }
            }
        }

        public void LoadModel(XnaModel model, Texture2D texture)
        {
            xnaModel = model;
            XNATexture = texture;
            ModelTransforms = new Matrix[xnaModel.Bones.Count];
            xnaModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
        }
    }
}
