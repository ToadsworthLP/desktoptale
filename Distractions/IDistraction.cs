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
        
        void Initialize();
        void LoadContent(ContentManager contentManager);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}