using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Martlet : Character
    {
        public Martlet(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Idle_Up"),    
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Idle_Down"),    
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Idle_Left"),    
                1
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Walk_Up"),
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Walk_Down"),
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Walk_Left"),
                4
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Run_Up"),    
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Run_Down"),    
                contentManager.Load<Texture2D>("Included/Martlet/Spr_Martlet_Run_Left"),    
                6
            );
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            RunSprite = runSprite;
        }
    }
}