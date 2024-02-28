using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public class AnimatedSprite
{
    public bool Playing { get; private set; } = false;
    public bool Loop { get; set; } = false;
    public double Framerate { get; set; }
    public Texture2D CurrentFrame => frames[CurrentFrameIndex];
    public int CurrentFrameIndex { get; set; }

    private IList<Texture2D> frames;
    private bool justStarted;
    private TimeSpan nextFrameUpdate;

    public AnimatedSprite()
    {
        frames = new List<Texture2D>();
        CurrentFrameIndex = 0;
    }

    public void Add(Texture2D texture)
    {
        frames.Add(texture);
    }
    
    public void Add(params Texture2D[] textures)
    {
        foreach (var texture in textures)
        {
            frames.Add(texture);
        }
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
            if (justStarted)
            {
                nextFrameUpdate = gameTime.TotalGameTime + TimeSpan.FromSeconds(1 / Framerate);
                justStarted = false;
            }
            
            if (nextFrameUpdate < gameTime.TotalGameTime)
            {
                nextFrameUpdate = gameTime.TotalGameTime + TimeSpan.FromSeconds(1 / Framerate);

                if (CurrentFrameIndex < frames.Count - 1)
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
        Rectangle? sourceRectangle, 
        Color color, 
        float rotation, 
        Vector2 origin, 
        Vector2 scale, 
        SpriteEffects effects, 
        float layerDepth)
    {
        spriteBatch.Draw(frames[CurrentFrameIndex], position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }
}