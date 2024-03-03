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
        
        OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite();
        idleSprite.Loop = false;
        idleSprite.Framerate = 0;
        idleSprite.Orientation = Orientation.Down;
        idleSprite.LoadOrientedSpriteFrames(contentManager, "Characters/Clover/Spr_Clover_Idle");
        IdleSprite = idleSprite;

        OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite();
        walkSprite.Loop = true;
        walkSprite.Framerate = 5;
        walkSprite.Orientation = Orientation.Down;
        WalkSprite = walkSprite;
        
        walkSprite.LoadOrientedSpriteFrames(contentManager, Orientation.Up, 
            "Characters/Clover/Spr_Clover_Idle_Up",
            "Characters/Clover/Spr_Clover_Walk_0_Up",
            "Characters/Clover/Spr_Clover_Idle_Up",
            "Characters/Clover/Spr_Clover_Walk_1_Up"
        );
        
        walkSprite.LoadOrientedSpriteFrames(contentManager, Orientation.Down, 
            "Characters/Clover/Spr_Clover_Idle_Down",
            "Characters/Clover/Spr_Clover_Walk_0_Down",
            "Characters/Clover/Spr_Clover_Idle_Down",
            "Characters/Clover/Spr_Clover_Walk_1_Down"
        );
        
        walkSprite.LoadOrientedSpriteFrames(contentManager, Orientation.Left, 
            "Characters/Clover/Spr_Clover_Idle_Left",
            "Characters/Clover/Spr_Clover_Walk_Left"
        );
        
        walkSprite.LoadOrientedSpriteFrames(contentManager, Orientation.Right, 
            "Characters/Clover/Spr_Clover_Idle_Right",
            "Characters/Clover/Spr_Clover_Walk_Right"
        );
        
        OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite();
        runSprite.Loop = true;
        runSprite.Framerate = 10;
        runSprite.Orientation = Orientation.Down;
        RunSprite = runSprite;
        
        runSprite.LoadOrientedSpriteFrames(contentManager, 
            "Characters/Clover/Spr_Clover_Run_0",
            "Characters/Clover/Spr_Clover_Run_1",
            "Characters/Clover/Spr_Clover_Run_2",
            "Characters/Clover/Spr_Clover_Run_3",
            "Characters/Clover/Spr_Clover_Run_4",
            "Characters/Clover/Spr_Clover_Run_5"
        );
    }
}