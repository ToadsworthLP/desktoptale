using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public class OrientedAnimatedSprite : IAnimatedSprite
    {
        public Orientation Orientation
        {
            get => _orientation;
            set
        {
            if(_orientation == value) return;
            
            UpdateOrientation(_orientation, value);
            _orientation = value;
        }
        }

        public bool KeepProgressOnDirectionChange { get; set; } = false;

        public bool Playing => SpriteForOrientation(Orientation).Playing;
        public bool Loop
        {
            get => SpriteForOrientation(Orientation).Loop;
            set
        {
            upSprite.Loop = value;
            downSprite.Loop = value;
            leftSprite.Loop = value;
            rightSprite.Loop = value;
        }
        }
        public double Framerate
        {
            get => SpriteForOrientation(Orientation).Framerate;
            set
        {
            upSprite.Framerate = value;
            downSprite.Framerate = value;
            leftSprite.Framerate = value;
            rightSprite.Framerate = value;
        }
        }
    
        public int StartFrame {         
            get => SpriteForOrientation(Orientation).StartFrame;
            set
        {
            upSprite.StartFrame = value;
            downSprite.StartFrame = value;
            leftSprite.StartFrame = value;
            rightSprite.StartFrame = value;
        }        }
    
        public int CurrentFrameIndex { 
            get => SpriteForOrientation(Orientation).CurrentFrameIndex;
            set => SpriteForOrientation(Orientation).CurrentFrameIndex = value;
        }

        public Point FrameSize => SpriteForOrientation(Orientation).FrameSize;
    
        private AnimatedSprite upSprite, downSprite, leftSprite, rightSprite;
        private Orientation _orientation;
        private bool flipRightSprite = false;

        public OrientedAnimatedSprite(Texture2D upSpritesheet, int upFrameCount, Texture2D downSpritesheet, int downFrameCount, Texture2D leftSpritesheet, int leftFrameCount, Texture2D rightSpritesheet, int rightFrameCount)
    {
        upSprite = new AnimatedSprite(upSpritesheet, upFrameCount);
        downSprite = new AnimatedSprite(downSpritesheet, downFrameCount);
        leftSprite = new AnimatedSprite(leftSpritesheet, leftFrameCount);
        rightSprite = new AnimatedSprite(rightSpritesheet, rightFrameCount);

        Orientation = Orientation.Down;
    }
    
        public OrientedAnimatedSprite(Texture2D upSpritesheet, int upFrameCount, Texture2D downSpritesheet, int downFrameCount, Texture2D leftSpritesheet, int leftFrameCount)
    {
        upSprite = new AnimatedSprite(upSpritesheet, upFrameCount);
        downSprite = new AnimatedSprite(downSpritesheet, downFrameCount);
        leftSprite = new AnimatedSprite(leftSpritesheet, leftFrameCount);
        rightSprite = new AnimatedSprite(leftSpritesheet, leftFrameCount);

        Orientation = Orientation.Down;
        flipRightSprite = true;
    }
    
        public OrientedAnimatedSprite(Texture2D upSpritesheet, Texture2D downSpritesheet, Texture2D leftSpritesheet, Texture2D rightSpritesheet, int frameCount)
    {
        upSprite = new AnimatedSprite(upSpritesheet, frameCount);
        downSprite = new AnimatedSprite(downSpritesheet, frameCount);
        leftSprite = new AnimatedSprite(leftSpritesheet, frameCount);
        rightSprite = new AnimatedSprite(rightSpritesheet, frameCount);
        
        Orientation = Orientation.Down;
    }
    
        public OrientedAnimatedSprite(Texture2D upSpritesheet, Texture2D downSpritesheet, Texture2D leftSpritesheet, int frameCount)
    {
        upSprite = new AnimatedSprite(upSpritesheet, frameCount);
        downSprite = new AnimatedSprite(downSpritesheet, frameCount);
        leftSprite = new AnimatedSprite(leftSpritesheet, frameCount);
        rightSprite = new AnimatedSprite(leftSpritesheet, frameCount);
        
        Orientation = Orientation.Down;
        flipRightSprite = true;
    }

        public OrientedAnimatedSprite(AnimatedSprite upSprite, AnimatedSprite downSprite, AnimatedSprite leftSprite, AnimatedSprite rightSprite)
    {
        this.upSprite = upSprite;
        this.downSprite = downSprite;
        this.leftSprite = leftSprite;
        this.rightSprite = rightSprite;
        
        Orientation = Orientation.Down;
    }

        public void Play()
    {
        SpriteForOrientation(Orientation).Play();
    }

        public void Pause()
    {
        SpriteForOrientation(Orientation).Pause();
    }

        public void Stop()
    {
        SpriteForOrientation(Orientation).Stop();
    }

        public void Update(GameTime gameTime)
    {
        SpriteForOrientation(Orientation).Update(gameTime);
    }
    
        public void Draw(SpriteBatch spriteBatch,
            Vector2 position,
            Color color, 
            float rotation, 
            Vector2 origin, 
            Vector2 scale, 
            SpriteEffects effects, 
            float layerDepth)
    {
        if (flipRightSprite && _orientation == Orientation.Right) effects |= SpriteEffects.FlipHorizontally;
        SpriteForOrientation(Orientation).Draw(spriteBatch, position, color, rotation, origin, scale, effects, layerDepth);
    }

        private void UpdateOrientation(Orientation oldValue, Orientation newValue)
    {
        AnimatedSprite oldSprite = SpriteForOrientation(oldValue);
        AnimatedSprite newSprite = SpriteForOrientation(newValue);
        
        if(oldSprite.Playing) newSprite.Play();
        oldSprite.Stop();

        if (KeepProgressOnDirectionChange)
        {
            newSprite.CurrentFrameIndex = oldSprite.CurrentFrameIndex;
        }
    }
    
        private AnimatedSprite SpriteForOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Up:
                return upSprite;
                break;
            case Orientation.Down:
                return downSprite;
                break;
            case Orientation.Left:
                return leftSprite;
                break;
            case Orientation.Right:
                return rightSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    }
}