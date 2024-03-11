using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public class SpriteCache
    {
        private IDictionary<string, Texture2D> cache;
        private GraphicsDevice graphicsDevice;

        public SpriteCache(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            cache = new Dictionary<string, Texture2D>();
        }

        public Texture2D Get(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path];
            }

            Texture2D texture = Texture2D.FromFile(graphicsDevice, path);
            cache.Add(path, texture);

            return texture;
        }
    }
}