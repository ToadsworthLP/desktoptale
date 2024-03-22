using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Lancer : Character
    {
        public Lancer(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Lancer/Spr_Lancer_Idle_Up"),  
                contentManager.Load<Texture2D>("Included/Lancer/Spr_Lancer_Idle_Down"), 
                contentManager.Load<Texture2D>("Included/Lancer/Spr_Lancer_Idle_Left"),
                1
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;
            WalkSprite = idleSprite;
            RunSprite = idleSprite;
        }
    }
}