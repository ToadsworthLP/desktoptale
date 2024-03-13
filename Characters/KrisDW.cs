using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class KrisDW : Character
    {
        public KrisDW(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Up"),    
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Left"),
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Right"),
                4
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Up"),    
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Left"),
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Right"),
                4
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Up"),    
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Down"), 
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Left"),
                contentManager.Load<Texture2D>("Included/KrisDW/Spr_KrisDW_Walk_Right"),
                4
            );
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;
        }
    }
}