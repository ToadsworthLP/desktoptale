using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Effect = Microsoft.Xna.Framework.Graphics.Effect;

namespace Desktoptale
{
    public class CharacterSpriteEffect : Effect
    {
        private EffectParameter matrixParam;
        private Viewport lastViewport;
        private Matrix projection;
        
        public CharacterSpriteEffect(GraphicsDevice device)
            : base(device, File.ReadAllBytes("Content/Included/Common/CharacterSpriteEffect.dx11.mgfxo"))
        {
            CacheEffectParameters();
        }
        
        public Matrix? TransformMatrix { get; set; }
        
        protected CharacterSpriteEffect(CharacterSpriteEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();
        }
        
        public override Effect Clone()
        {
            return new CharacterSpriteEffect(this);
        }
        
        void CacheEffectParameters()
        {
            matrixParam = Parameters["MatrixTransform"];
        }

        protected override void OnApply()
        {
            var vp = GraphicsDevice.Viewport;
            if (vp.Width != lastViewport.Width || (vp.Height != lastViewport.Height))
            {
                Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, -1, out projection);

                if (GraphicsDevice.UseHalfPixelOffset)
                {
                    projection.M41 += -0.5f * projection.M11;
                    projection.M42 += -0.5f * projection.M22;
                }

                lastViewport = vp;
            }

            if (TransformMatrix.HasValue)
                matrixParam.SetValue(TransformMatrix.GetValueOrDefault() * projection);
            else
                matrixParam.SetValue(projection);
        }
    }
}