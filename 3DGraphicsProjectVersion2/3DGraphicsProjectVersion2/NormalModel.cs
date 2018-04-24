using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sample;

namespace _3DGraphicsProjectVersion2
{
    class NormalModel : CustomEffectModel
    {
        public class NormalMaterial : Material
        {
            public Texture2D NormalTexture { get; set; }
            public Color AmbientColour { get; set; }
            public Color DiffuseColour { get; set; }
            public Color LightColour { get; set; }
            public Texture2D Texture { get; set; }
            public float Attenuation { get; set; }
            public float FallOff { get; set; }
            public Vector3 Position { get; set; }

            public override void SetEffectParameters(Effect effect)
            {
                effect.Parameters["AmbientColour"].SetValue(AmbientColour.ToVector3());
                effect.Parameters["DiffuseColour"].SetValue(DiffuseColour.ToVector3());
                effect.Parameters["LightColour"].SetValue(LightColour.ToVector3());
                effect.Parameters["Attenuation"].SetValue(Attenuation);
                effect.Parameters["FallOff"].SetValue(FallOff);
                effect.Parameters["Position"].SetValue(Position);
                effect.Parameters["Texture"].SetValue(Texture);
                effect.Parameters["NormalTexture"].SetValue(NormalTexture);

                base.SetEffectParameters(effect);
            }
        }

        public NormalModel(string asset, Vector3 position) : base(asset, position)
        {

        }

        public override void LoadContent()
        {
            customEffect = GameUtilities.Content.Load<Effect>("effects\\");

            material = new NormalMaterial()
            {
                AmbientColour = new Color(.15f, .15f, .15f),
                DiffuseColour = Color.White,
                LightColour = Color.White,
                Position = new Vector3(0, 0, 0),
                FallOff = 2,
                Attenuation = 20,
                Texture = GameUtilities.Content.Load<Texture2D>("textures\\fur"),
                NormalTexture = GameUtilities.Content.Load<Texture2D>("normalMaps\\FurNormalMap")
            };

            base.LoadContent();
        }

        public override void Update()
        {
            var thisMaterial = (material as NormalMaterial);

            DebugEngine.AddBoundingSphere(new BoundingSphere(thisMaterial.Position, thisMaterial.Attenuation), Color.White);

            if(InputEngine.IsKeyHeld(Keys.Left))
            {
                thisMaterial.Position -= new Vector3(0.5f, 0, 0);
            }
            else if (InputEngine.IsKeyHeld(Keys.Right))
            {
                thisMaterial.Position += new Vector3(0.5f, 0, 0);
            }

            if (InputEngine.IsKeyHeld(Keys.Up))
            {
                thisMaterial.Position += new Vector3(0, 0.5f, 0);
            }
            else if (InputEngine.IsKeyHeld(Keys.Down))
            {
                thisMaterial.Position -= new Vector3(0, 0.5f, 0);
            }

            if (InputEngine.IsKeyHeld(Keys.Add))
            {
                thisMaterial.Attenuation += 0.5f;
            }
            else if (InputEngine.IsKeyHeld(Keys.Subtract))
            {
                thisMaterial.Attenuation -= 0.5f;
            }

            base.Update();
        }
    }
}
