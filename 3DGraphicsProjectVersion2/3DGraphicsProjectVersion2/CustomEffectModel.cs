using Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGraphicsProjectVersion2
{
    public class CustomEffectModel : SimpleModel
    {
        public Effect customEffect { get; set; }
        public Material material { get; set; }

        public CustomEffectModel(string asset, Vector3 position) : base("", asset,position)
        {
            material = new Material();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if(customEffect != null && Model != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        part.Effect = customEffect;
                    }
                }
            }
        }

        public override void Draw(Camera camera)
        {
            foreach (var mesh in Model.Meshes)
            {
                foreach(var part in mesh.MeshParts)
                {
                    part.Effect.Parameters["World"].SetValue(BoneTransforms[mesh.ParentBone.Index] * World);
                    part.Effect.Parameters["View"].SetValue(camera.View);
                    part.Effect.Parameters["Projection"].SetValue(camera.Projection);

                    if(material != null)
                    {
                        material.SetEffectParameters(part.Effect);
                    }
                }
                mesh.Draw();
            }
        }
    }
}
