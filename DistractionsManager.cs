using System;
using System.Collections.Generic;
using Desktoptale.Distractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public class DistractionsManager
    {
        private ContentManager contentManager;
        private GameWindow window;

        private ISet<IDistraction> distractions;
        
        public DistractionsManager(ContentManager contentManager, GameWindow window)
        {
            this.contentManager = contentManager;
            this.window = window;

            distractions = new HashSet<IDistraction>();
        }
        
        public void Initialize()
        {
            IDistraction dist = new BaseBoneDistraction();
            dist.Position = new Vector2(window.ClientBounds.Width / 2f, window.ClientBounds.Height / 2f);
            dist.Rotation = MathHelper.ToRadians(90f);
            AddDistraction(dist);
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (IDistraction distraction in distractions)
            {
                distraction.Update(gameTime);
            }
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IDistraction distraction in distractions)
            {
                distraction.Draw(gameTime, spriteBatch);
            }
        }

        private void AddDistraction(IDistraction distraction)
        {
            distraction.LoadContent(contentManager);
            distraction.Initialize();
            
            distractions.Add(distraction);
        }
    }
}