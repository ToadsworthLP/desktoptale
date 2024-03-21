using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class StandardCharacter : Character
    {
        private string name;
        private bool hasRightSprite;
        private int upFrames, downFrames, leftFrames, rightFrames;

        public StandardCharacter(CharacterCreationContext characterCreationContext, string name, bool hasRightSprite, int upFrames = 4, int downFrames = 4, int leftFrames = 4, int rightFrames = 4) : base(characterCreationContext)
        {
            this.name = name;
            this.hasRightSprite = hasRightSprite;
            this.upFrames = upFrames;
            this.downFrames = downFrames;
            this.leftFrames = leftFrames;
            this.rightFrames = rightFrames;
        }
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite;

            if (hasRightSprite)
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"), rightFrames
                );
            }
            else
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,  
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames
                );
            }
            
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite;
            
            if (hasRightSprite)
            {
                walkSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"), rightFrames
                );
            }
            else
            {
                walkSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames
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
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Right"), rightFrames
                );
            }
            else
            {
                runSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Up"), upFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Down"), downFrames,
                    contentManager.Load<Texture2D>($"Included/{name}/Spr_{name}_Walk_Left"), leftFrames
                );
            }
            
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;
        }
    }
}