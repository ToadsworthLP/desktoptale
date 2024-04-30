using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Distractions
{
    public class BoneDistraction : IDistraction
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public virtual bool Disposed { get; set; }

        private static readonly string[] BoneTextures = {
            "Included/Distractions/Spr_Distraction_Bone_VeryShort",
            "Included/Distractions/Spr_Distraction_Bone_Short",
            "Included/Distractions/Spr_Distraction_Bone_Medium",
            "Included/Distractions/Spr_Distraction_Bone_Long"
        };

        protected BoneLength boneLength;
        protected Texture2D boneTexture;
        
        private Vector2 origin;
        
        public enum BoneLength { VeryShort = 0, Short = 1, Medium = 2, Long = 3 }
        
        public BoneDistraction(BoneLength length)
        {
            boneLength = length;
        }


        public virtual void Initialize()
        {
            
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            boneTexture = contentManager.Load<Texture2D>(BoneTextures[(int)boneLength]);
            origin = new Vector2(boneTexture.Width / 2f, boneTexture.Height / 2f);
        }

        public virtual void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle screenRectangle)
        {
            spriteBatch.Draw(boneTexture, Position, null, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}