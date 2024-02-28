using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Desktoptale;

public interface IGameObject
{
    void LoadContent(ContentManager contentManager);
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
}