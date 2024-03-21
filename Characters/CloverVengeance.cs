using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class CloverVengeance : Character
    {
        public CloverVengeance(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
        
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Up"), 4,
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Down"), 4,
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Left"), 2
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Up"), 4,
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Down"), 4,
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Walk_Left"), 2
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Run_Up"),    
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Run_Down"),    
                contentManager.Load<Texture2D>("Included/CloverVengeance/Spr_CloverVengeance_Run_Left"),    
                6
            );
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;
        }
    }
}