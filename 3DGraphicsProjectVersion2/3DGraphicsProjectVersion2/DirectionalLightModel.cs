using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sample;

namespace _3DGraphicsProjectVersion2
{
    public class DirectionalLightModel : CustomEffectModel
    {
        public class DirectionalMaterial : Material
        {
            public Color AmbientColour { get; set; }
            public Color DiffuseColour { get; set; }
            public Color LightColour { get; set; }

            public Vector3 Direction { get; set; }
            public Texture2D Texture { get; set; }

            public override void SetEffectParameters(Effect effect)
            {
                effect.Parameters["AmbientColour"].SetValue(AmbientColour.ToVector3());
                effect.Parameters["DiffuseColour"].SetValue(DiffuseColour.ToVector3());
                effect.Parameters["LightColour"].SetValue(LightColour.ToVector3());
                effect.Parameters["Direction"].SetValue(Direction);
                effect.Parameters["Texture"].SetValue(Texture);

                base.SetEffectParameters(effect);
            }
        }

        public DirectionalLightModel(string asset, Vector3 position)
            : base(asset, position)
        {
        }

        public override void LoadContent()
        {
            material = new DirectionalMaterial()
            {
                AmbientColour = Color.DarkGray,
                DiffuseColour = Color.White,
                LightColour = Color.White,
                Direction = Vector3.Up,
                Texture = GameUtilities.Content.Load<Texture2D>("Textures\\sand")
            };

            customEffect = GameUtilities.Content.Load<Effect>("Effects\\DirectionalLight");

            base.LoadContent();
        }
    }
}
