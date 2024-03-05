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
        OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Idle_Up"),    
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Idle_Down"),    
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Idle_Left"),    
            1
        );
        idleSprite.Loop = false;
        idleSprite.Framerate = 0;
        IdleSprite = idleSprite;

        OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Walk_Up"),
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Walk_Down"),
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Walk_Left"),
            4
        );
        walkSprite.Loop = true;
        walkSprite.Framerate = 5;
        walkSprite.StartFrame = 1;
        WalkSprite = walkSprite;

        OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Run_Up"),    
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Run_Down"),    
            contentManager.Load<Texture2D>("Characters/Ceroba/Spr_Ceroba_Run_Left"),    
            6
        );
        runSprite.Loop = true;
        runSprite.Framerate = 10;
        runSprite.StartFrame = 1;
        RunSprite = runSprite;
    }
}