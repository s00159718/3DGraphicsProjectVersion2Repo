using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DGraphicsProjectVersion2
{
    class MeshTag
    {
        public Vector3 Color;
        public Texture2D Texture;
        public float SpecularPower;
        public Effect CachedEffect;

        public MeshTag() { }

        public MeshTag(Vector3 color, Texture2D texture, float specularpower)
        {
            Color = color;
            Texture = texture;
            SpecularPower = specularpower;
        }
    }
}
