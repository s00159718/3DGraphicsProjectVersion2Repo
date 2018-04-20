using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sample;

namespace _3DGraphicsProjectVersion2
{
    class PointLightModel : CustomEffectModel
    {
        public class PointLightMaterial : Material
        {
            public Vector3 Position { get; set; }
            public Color LightColour { get; set; }
            public Color DiffuseColour { get; set; }
            public float LightAttenuation { get; set; }
            public Color AmbientColour { get; set; }
            public float Attenuation { get; set; }
            public float FallOff { get; set; }
            public Texture2D Texture { get; set; }

            public override void SetEffectParameters(Effect effect)
            {
                if (effect.Parameters["LightPosition"] != null)
                    effect.Parameters["LightPosition"].SetValue(Position);

                if (effect.Parameters["LightColour"] != null)
                    effect.Parameters["LightColour"].SetValue(LightColour.ToVector3());

                if (effect.Parameters["LightAttenuation"] != null)
                    effect.Parameters["LightAttenuation"].SetValue(Attenuation);

                if (effect.Parameters["DiffuseColour"] != null)
                    effect.Parameters["DiffuseColour"].SetValue(DiffuseColour.ToVector3());

                if (effect.Parameters["AmbientLightColour"] != null)
                    effect.Parameters["AmbientLightColour"].SetValue(AmbientColour.ToVector3());

                if (effect.Parameters["LightAttenuation"] != null)
                    effect.Parameters["LightAttenuation"].SetValue(Attenuation);

                if (effect.Parameters["LightFalloff"] != null)
                    effect.Parameters["LightFalloff"].SetValue(FallOff);

                base.SetEffectParameters(effect);
            }
        }

        public PointLightModel(string asset, Vector3 position) : base(asset, position)
        {

        }

        public override void LoadContent()
        {
            customEffect = GameUtilities.Content.Load<Effect>("effects\\FinalLightEffect");

            customEffect.Parameters["Texture"].SetValue(GameUtilities.Content.Load<Texture2D>("textures/camo"));
            customEffect.Parameters["TextureEnabled"].SetValue(true);

            base.LoadContent();
        }
    }
}
