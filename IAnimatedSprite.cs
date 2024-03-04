using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public interface IAnimatedSprite
{
    public bool Playing { get;}
    public bool Loop { get; set; }
    public double Framerate { get; set; }
    public int CurrentFrameIndex { get; set; }
    public Point FrameSize { get; }

    public void Play();
    public void Pause();
    public void Stop();
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects effects,
        float layerDepth);
}