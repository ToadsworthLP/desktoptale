using Desktoptale.States.Common;
using Desktoptale.States.Soul;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Soul : Character
    {
        protected override IState<Character> InitialState => new SoulIdleState();
        
        private Color color;
        private bool flip;
        
        public Soul(CharacterCreationContext characterCreationContext, Color color, bool flip = false) : base(characterCreationContext)
        {
            this.color = color;
            this.flip = flip;
        }
    
        public override void LoadContent(ContentManager contentManager)
        {
            AnimatedSprite idleSprite = new AnimatedSprite(contentManager.Load<Texture2D>("Included/Soul/Spr_Soul"), 1);
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;
            WalkSprite = idleSprite;
            RunSprite = idleSprite;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            IdleState = InitialState;
            WalkState = new WalkState(100f, false);
            RunState = new RunState(50f, false);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height);
            Vector2 origin = new Vector2(CurrentSprite.FrameSize.X / 2f, CurrentSprite.FrameSize.Y);
            CurrentSprite.Draw(spriteBatch, center, color, 0, origin, Scale, flip ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
        }
    }
}