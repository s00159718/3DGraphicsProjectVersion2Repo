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

            if (Model != null)
            {
                GenerateMeshTag();

                if (customEffect != null)
                {
                    foreach (var mesh in Model.Meshes)
                        foreach (var part in mesh.MeshParts)
                            part.Effect = customEffect;
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

        public void GenerateMeshTag()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    MeshTag tag = new MeshTag();

                    if (part.Effect is BasicEffect)
                    {
                        tag.Color = (part.Effect as BasicEffect).DiffuseColor;
                        tag.Texture = (part.Effect as BasicEffect).Texture;
                        tag.SpecularPower = (part.Effect as BasicEffect).SpecularPower;

                        part.Tag = tag;
                    }
                }
            }
        }

        public virtual void CacheEffect()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    (part.Tag as MeshTag).CachedEffect = part.Effect;
                }
        }

        public virtual void RestoreEffect()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (part.Tag != null)
                    {
                        if ((part.Tag as MeshTag).CachedEffect != null)
                            part.Effect = (part.Tag as MeshTag).CachedEffect;
                    }
                }
        }

        public virtual void SetModelEffect(Effect effect, bool copyEffect)
        {
            CacheEffect();

            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect toBeSet = effect;

                    if (copyEffect)
                    {
                        toBeSet = effect.Clone();
                    }

                    var tag = (part.Tag as MeshTag);

                    if (tag.Texture != null)
                    {
                        SetEffectParameter(toBeSet, "Texture", tag.Texture);
                        SetEffectParameter(toBeSet, "TextureEnabled", true);
                    }
                    else
                        SetEffectParameter(toBeSet, "TextureEnabled", false);

                    SetEffectParameter(toBeSet, "DiffuseColor", tag.Color);
                    SetEffectParameter(toBeSet, "SpecularPower", tag.SpecularPower);

                    part.Effect = toBeSet;
                }
        }

        public virtual void SetEffectParameter(Effect effect, string paramName, object value)
        {
            if (effect.Parameters[paramName] == null)
                return;

            if (value is Vector3)
            {
                effect.Parameters[paramName].SetValue((Vector3)value);
            }
            else if (value is Matrix)
            {
                effect.Parameters[paramName].SetValue((Matrix)value);
            }
            else if (value is bool)
            {
                effect.Parameters[paramName].SetValue((bool)value);
            }
            else if (value is Texture2D)
            {
                effect.Parameters[paramName].SetValue((Texture2D)value);
            }
            else if (value is float)
            {
                effect.Parameters[paramName].SetValue((float)value);
            }
            else if (value is int)
            {
                effect.Parameters[paramName].SetValue((int)value);
            }
        }
    }
}
