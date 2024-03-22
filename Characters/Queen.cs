using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Queen : Character
    {
        public Queen(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Up"),  
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Left"),
                4
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Up"),  
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Left"),
                4
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Up"),  
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/Queen/Spr_Queen_Run_Left"),
                4
            );
            runSprite.Loop = true;
            runSprite.Framerate = 9;
            runSprite.StartFrame = 0;
            RunSprite = runSprite;
        }
    }
}