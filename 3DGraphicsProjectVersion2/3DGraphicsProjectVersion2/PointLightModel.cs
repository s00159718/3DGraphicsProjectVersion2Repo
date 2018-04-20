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

            public override void SetEffectParameters(Effect effect)
            {
                effect.Parameters["LightPosition"].SetValue(Position);
                effect.Parameters["LightColour"].SetValue(LightColour.ToVector3());
                effect.Parameters["LightAttenuation"].SetValue(LightAttenuation);
                effect.Parameters["DiffuseColour"].SetValue(DiffuseColour.ToVector3());

                base.SetEffectParameters(effect);
            }
        }

        public PointLightModel(string asset, Vector3 position) : base(asset, position)
        {

        }

        public override void LoadContent()
        {
            customEffect = GameUtilities.Content.Load<Effect>("PointLight");

            material = new PointLightMaterial()
            {
                LightColour = Color.White,
                DiffuseColour = Color.White,
                LightAttenuation = 20,
                Position = World.Translation + new Vector3(0, 10, 0)
            };

            base.LoadContent();
        }
    }
}
