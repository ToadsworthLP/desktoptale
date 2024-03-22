using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Spamton : Character
    {
        public Spamton(CharacterCreationContext characterCreationContext) : base(characterCreationContext)
        {
        }

        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Spamton/Spr_Spamton_Up"),
                contentManager.Load<Texture2D>("Included/Spamton/Spr_Spamton_Left"),
                contentManager.Load<Texture2D>("Included/Spamton/Spr_Spamton_Left"),
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