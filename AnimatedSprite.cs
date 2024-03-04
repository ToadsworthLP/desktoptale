using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public class AnimatedSprite : IAnimatedSprite
{
    public bool Playing { get; private set; }
    public bool Loop { get; set; }
    public double Framerate { get; set; }
    public int CurrentFrameIndex { get; set; }
    public Point FrameSize { get; }

    //private IList<Texture2D> frames;
    private Texture2D spritesheet;
    private int frameCount;
    private bool justStarted;
    private TimeSpan nextFrameUpdate;

    public AnimatedSprite(Texture2D spritesheet, int frameCount)
    {
        this.spritesheet = spritesheet;
        this.frameCount = frameCount;
        FrameSize = new Point(spritesheet.Width / frameCount, spritesheet.Height);
    }
    
    public void Play()
    {
        Playing = true;
        justStarted = true;
    }

    public void Pause()
    {
        Playing = false;
    }

    public void Stop()
    {
        Playing = false;
        CurrentFrameIndex = 0;
    }

    public void Update(GameTime gameTime)
    {
        if (Playing)
        {
            TimeSpan nextScheduledUpdate = Framerate == 0 ? TimeSpan.MaxValue : gameTime.TotalGameTime + TimeSpan.FromSeconds(1 / Framerate);
            
            if (justStarted)
            {
                nextFrameUpdate = nextScheduledUpdate;
                justStarted = false;
            }
            
            if (nextFrameUpdate < gameTime.TotalGameTime)
            {
                nextFrameUpdate = nextScheduledUpdate;

                if (CurrentFrameIndex < frameCount - 1)
                {
                    CurrentFrameIndex++;
                }
                else
                {
                    if (Loop) CurrentFrameIndex = 0;
                }
            }
        }
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
        Rectangle sourceRectangle = new Rectangle(CurrentFrameIndex * FrameSize.X, 0, FrameSize.X, FrameSize.Y);
        spriteBatch.Draw(spritesheet, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }
}