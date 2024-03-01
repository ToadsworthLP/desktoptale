using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Desktoptale;

public interface IGameObject
{
    void Initialize();
    void LoadContent(ContentManager contentManager);
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
}