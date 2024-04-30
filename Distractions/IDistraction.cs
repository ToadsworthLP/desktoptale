using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Distractions
{
    public interface IDistraction
    {
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        bool Disposed { get; }
        
        void Initialize();
        void LoadContent(ContentManager contentManager);
        void Update(GameTime gameTime, Rectangle screenRectangle);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle screenRectangle);
    }
}