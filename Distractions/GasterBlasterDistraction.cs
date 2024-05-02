using System;
using Desktoptale.States.GasterBlaster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class GasterBlasterDistraction : IDistraction
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public bool Disposed { get; set; }

        public float BlasterOpacity = 1f;
        public float BeamWidth = 1f;
        public float BeamOpacity = 1f;

        public AnimatedSprite BlasterSprite;
        
        private Texture2D beamTexture;
        private Texture2D beamTipTexture;
        
        private Vector2 blasterOrigin;
        private Vector2 beamOrigin;
        private Vector2 beamTipOrigin;

        private StateMachine<GasterBlasterDistraction> stateMachine;

        private IState<GasterBlasterDistraction> initialWaitState;
        private IState<GasterBlasterDistraction> appearState;
        private IState<GasterBlasterDistraction> fireWaitState;
        private IState<GasterBlasterDistraction> fireState;

        private TimeSpan initialDelay;
        private Random random;
        private float depthOffset;

        public GasterBlasterDistraction(TimeSpan delay)
        {
            initialDelay = delay;
            random = new Random();
            depthOffset = random.NextFloat(0f, 0.00024f);
        }
        
        public void Initialize()
        {
            fireState = new BlasterFireState(TimeSpan.FromSeconds(1f));
            fireWaitState = new BlasterWaitState(fireState, TimeSpan.FromSeconds(0.8f));
            appearState = new BlasterAppearState(fireWaitState, Rotation, TimeSpan.FromSeconds(0.25f), 100f * Scale.Y);
            initialWaitState = new BlasterWaitState(appearState, initialDelay);
            
            stateMachine = new StateMachine<GasterBlasterDistraction>(this, initialWaitState);
            
            BlasterOpacity = 0f;
            BeamOpacity = 0f;
            BeamWidth = 0f;
        }

        public void LoadContent(ContentManager contentManager)
        {
            Texture2D blasterTexture = contentManager.Load<Texture2D>("Included/Distractions/Spr_Distraction_GasterBlaster");
            BlasterSprite = new AnimatedSprite(blasterTexture, 6);
            BlasterSprite.Framerate = 30;
            BlasterSprite.LoopPoint = 4;
            BlasterSprite.Loop = true;

            beamTexture = contentManager.Load<Texture2D>("Included/Distractions/Spr_Distraction_WhitePixel");
            beamTipTexture = contentManager.Load<Texture2D>("Included/Distractions/Spr_Distraction_BeamTip");
            
            blasterOrigin = new Vector2(BlasterSprite.FrameSize.X / 2f, BlasterSprite.FrameSize.Y * 0.8f);
            beamOrigin = new Vector2(0.5f, 0);
            beamTipOrigin = new Vector2(beamTipTexture.Width / 2f, beamTipTexture.Height);
        }

        public void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            stateMachine.Update(gameTime);
            BlasterSprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle screenRectangle)
        {
            Color blasterColor = new Color(BlasterOpacity, BlasterOpacity, BlasterOpacity, BlasterOpacity);
            BlasterSprite.Draw(spriteBatch, Position, blasterColor, Rotation, blasterOrigin, Scale, SpriteEffects.None, 0.00025f + depthOffset);

            Vector2 beamScale = new Vector2((Scale.X * BlasterSprite.FrameSize.X) * 0.6f * BeamWidth,
                MathHelper.Max(screenRectangle.Width, screenRectangle.Height));
            
            Vector2 beamTipScale = new Vector2((Scale.X * BlasterSprite.FrameSize.X * 0.6f * BeamWidth) / beamTipTexture.Width,
                Scale.Y * 2);

            Color beamColor = new Color(BeamOpacity, BeamOpacity, BeamOpacity, BeamOpacity);
            
            spriteBatch.Draw(beamTexture, Position, null, beamColor, Rotation, beamOrigin, beamScale, SpriteEffects.None, 0f + depthOffset);
            spriteBatch.Draw(beamTipTexture, Position, null, beamColor, Rotation, beamTipOrigin, beamTipScale, SpriteEffects.None, 0f + depthOffset);
        }
    }
}