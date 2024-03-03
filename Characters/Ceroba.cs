using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters;

public class Ceroba : Character
{
    public Ceroba(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager) : base(graphics, window, spriteBatch, inputManager)
    {
    }
    
    public override void LoadContent(ContentManager contentManager)
    {
        base.LoadContent(contentManager);
        
        OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite();
        idleSprite.Loop = false;
        idleSprite.Framerate = 0;
        idleSprite.Orientation = Orientation.Down;
        idleSprite.LoadOrientedSpriteFrames(contentManager, "Characters/Ceroba/Spr_Ceroba_Idle");
        IdleSprite = idleSprite;
    }
}