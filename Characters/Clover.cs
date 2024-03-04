using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters;

public class Clover : Character
{
    public Clover(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager) : base(graphics, window, spriteBatch, inputManager) {}

    public override void LoadContent(ContentManager contentManager)
    {
        base.LoadContent(contentManager);
        
        OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Up"),    
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Down"),    
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Left"),    
            1
        );
        idleSprite.Loop = false;
        idleSprite.Framerate = 0;
        IdleSprite = idleSprite;

        OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Up"), 4,
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Down"), 4,
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Left"), 2
        );
        walkSprite.Loop = true;
        walkSprite.Framerate = 5;
        WalkSprite = walkSprite;

        OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Up"),    
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Down"),    
            contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Left"),    
            6
        );
        runSprite.Loop = true;
        runSprite.Framerate = 10;
        RunSprite = runSprite;
    }
}