using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Distractions
{
    public class BaseBoneDistraction : IDistraction
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One * 3;
        
        private Texture2D bone;
        private Vector2 origin;
        
        public BaseBoneDistraction()
        {
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            bone = contentManager.Load<Texture2D>($"Included/Distractions/Spr_Distraction_Bone_Long");
            origin = new Vector2(bone.Width / 2f, bone.Height / 2f);
        }

        public virtual void Update(GameTime gameTime)
        {
            Rotation += gameTime.ElapsedGameTime.Milliseconds * 0.005f;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bone, Position, null, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}