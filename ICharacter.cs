using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public interface ICharacter : IDisposable, IPhysicsObject
    {
        CharacterProperties Properties { get; }
        TrackedWindow TrackedWindow { get; }
        
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Vector2 Scale { get; set; }
        
        void Initialize();
        void LoadContent(ContentManager contentManager);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}