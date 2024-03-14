using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class StandardCharacter : Character
    {
        private string name;
        private bool hasRightSprite;

        public StandardCharacter(CharacterCreationContext characterCreationContext, string name, bool hasRightSprite) : base(characterCreationContext)
        {
            this.name = name;
            this.hasRightSprite = hasRightSprite;
        }
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite;

            if (hasRightSprite)
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"),
                    4
                );
            }
            else
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    4
                );
            }
            
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite;
            
            if (hasRightSprite)
            {
                walkSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"),
                    4
                );
            }
            else
            {
                walkSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    4
                );
            }
            
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite;
            
            if (hasRightSprite)
            {
                runSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"),
                    4
                );
            }
            else
            {
                runSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"),    
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), 
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"),
                    4
                );
            }
            
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;
        }
    }
}