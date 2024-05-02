using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public interface IAnimatedSprite
    {
        bool Playing { get;}
        bool Loop { get; set; }
        int LoopPoint { get; set; }
        double Framerate { get; set; }
        int FrameCount { get; }
        int StartFrame { get; set; }
        int CurrentFrameIndex { get; set; }
        Point FrameSize { get; }

        void Play();
        void Pause();
        void Stop();
        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth);
    }
}