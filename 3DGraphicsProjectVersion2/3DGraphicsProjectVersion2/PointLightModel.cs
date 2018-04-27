using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sample;
using Microsoft.Xna.Framework.Input;

namespace _3DGraphicsProjectVersion2
{
    class PointLightModel : CustomEffectModel
    {
        public class PointLightMaterial : Material
        {
            public Vector3[] Position { get; set; }
            public Vector3[] LightColor { get; set; }
            public Color DiffuseColor { get; set; }
            public float[] Attenuation { get; set; }
            public Color AmbientColor { get; set; }
            public float[] FallOff { get; set; }
            public Texture2D Texture { get; set; }
            public Vector3[] SpecularColor { get; set; }

            public PointLightMaterial()
            {
                Position = new Vector3[3];
                LightColor = new Vector3[3];
                DiffuseColor = Color.White;
                Attenuation = new float[3];
                FallOff = new float[3];
                SpecularColor = new Vector3[3];

                Position[0] = new Vector3(20, 0, 0);
                Position[1] = new Vector3(0, 20, 0);
                Position[2] = new Vector3(0, 0, 20);

                LightColor[0] = Color.Red.ToVector3();
                LightColor[1] = Color.Green.ToVector3();
                LightColor[2] = Color.Blue.ToVector3();

                Attenuation[0] = 10;
                Attenuation[1] = 100;
                Attenuation[2] = 150;

                FallOff[0] = 2;
                FallOff[1] = 5;
                FallOff[2] = 2;

                SpecularColor[0] = new Vector3(0.5f, 0.1f, 0.1f);
                SpecularColor[1] = new Vector3(0.1f, 0.1f, 0.1f);
                SpecularColor[2] = new Vector3(0.1f, 0.1f, 0.1f);
            }

            public override void Update()
            {
                base.Update();
            }

            public override void SetEffectParameters(Effect effect)
            {
                if (effect.Parameters["Position"] != null)
                {
                    effect.Parameters["Position"].SetValue(Position);
                }
                if (effect.Parameters["LightColor"] != null)
                {
                    effect.Parameters["LightColor"].SetValue(LightColor);
                }
                if (effect.Parameters["Attenuation"] != null)
                {
                    effect.Parameters["Attenuation"].SetValue(Attenuation);
                }
                if (effect.Parameters["DiffuseColor"] != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(DiffuseColor.ToVector3());
                }
                if (effect.Parameters["AmbientColor"] != null)
                {
                    effect.Parameters["AmbientColor"].SetValue(AmbientColor.ToVector3());
                }
                if (effect.Parameters["Falloff"] != null)
                {
                    effect.Parameters["Falloff"].SetValue(FallOff);
                }
                if (effect.Parameters["SpecularColor"] != null)
                {
                    effect.Parameters["SpecularColor"].SetValue(SpecularColor);
                }
                base.SetEffectParameters(effect);
            }
        }

        public PointLightModel(string asset, Vector3 position) : base(asset, position)
        {

        }

        public override void LoadContent()
        {
            customEffect = GameUtilities.Content.Load<Effect>("effects\\PointLight");

            customEffect.Parameters["Texture"].SetValue(GameUtilities.Content.Load<Texture2D>("textures/camo"));
            customEffect.Parameters["TextureEnabled"].SetValue(true);

            base.LoadContent();
        }

        public override void Update()
        {
            if(InputEngine.IsKeyPressed(Keys.T) && (customEffect.Parameters["TextureEnabled"] == true))
            {
                customEffect.Parameters["TextureEnabled"].SetValue(false);
            }
            else if (InputEngine.IsKeyPressed(Keys.T) && (customEffect.Parameters["TextureEnabled"] == false))
            {
                customEffect.Parameters["TextureEnabled"].SetValue(true);
            }

            base.Update();
        }
    }
}
