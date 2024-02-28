using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public class OrientedAnimatedSprite
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

    public Texture2D CurrentFrame => SpriteForOrientation(Orientation).CurrentFrame;
    public int CurrentFrameIndex { 
        get => SpriteForOrientation(Orientation).CurrentFrameIndex;
        set => SpriteForOrientation(Orientation).CurrentFrameIndex = value;
    }

    private AnimatedSprite upSprite, downSprite, leftSprite, rightSprite;
    private Orientation _orientation;

    public OrientedAnimatedSprite()
    {
        upSprite = new AnimatedSprite();
        downSprite = new AnimatedSprite();
        leftSprite = new AnimatedSprite();
        rightSprite = new AnimatedSprite();
    }
    
    public void Add(Orientation orientation, Texture2D texture)
    {
        SpriteForOrientation(orientation).Add(texture);
    }
    
    public void Add(Orientation orientation, params Texture2D[] textures)
    {
        foreach (var texture in textures)
        {
            SpriteForOrientation(orientation).Add(texture);
        }
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
        Rectangle? sourceRectangle, 
        Color color, 
        float rotation, 
        Vector2 origin, 
        Vector2 scale, 
        SpriteEffects effects, 
        float layerDepth)
    {
        SpriteForOrientation(Orientation).Draw(spriteBatch, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
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