using System;
using System.Collections.Generic;
using Desktoptale.Distractions;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale
{
    public class DistractionManager : IDistractionManager
    {
        public const int MaxDistractionLevel = 5;
            
        private const float ScaleDivisor = 500f;

        private static readonly float[] DistractionSpawnDelays = new[] { 256f, 64f, 16f, 4f, 1f };
        
        private ContentManager contentManager;
        private GameWindow window;

        private IList<IDistractionPattern> patterns;
        private ISet<IDistraction> distractions;
        private Vector2 scale;

        private IList<IDistraction> removalScheduled;

        private int distractionLevel;
        private Random random;
        private int lastDistractionIndex = -1;

        private TimeSpan nextScheduledDistractionTime = TimeSpan.MinValue;
        
        public DistractionManager(ContentManager contentManager, GameWindow window)
        {
            this.contentManager = contentManager;
            this.window = window;

            patterns = new List<IDistractionPattern>();
            distractions = new HashSet<IDistraction>();
            removalScheduled = new List<IDistraction>();

            random = new Random();

            MessageBus.Subscribe<SetDistractionLevelMessage>(OnSetDistractionLevelMessage);
        }
        
        public void Initialize()
        {
            int smallerDimension = Math.Min(window.ClientBounds.Width, window.ClientBounds.Height);
            float factor = (float)Math.Round(smallerDimension / ScaleDivisor);
            scale = new Vector2(factor, factor);

            patterns.Add(new UpDownOppositeBonesPattern(10, 300f, 120f * scale.Y, 100 * scale.X));
            patterns.Add(new LeftRightOppositeBonesPattern(10, 300f, 120f * scale.X, 100 * scale.Y));
            patterns.Add(new GasterBlasterPattern());
        }
        
        public void Update(GameTime gameTime)
        {
            if (distractionLevel > 0)
            {
                if (nextScheduledDistractionTime < gameTime.TotalGameTime)
                {
                    int index = random.Next(0, patterns.Count);
                    if (index == lastDistractionIndex) index = (index + 1) % patterns.Count;
                    lastDistractionIndex = index;
                    float waitMultiplier = patterns[index].Spawn(this, window.ClientBounds);
                    
                    float delay = DistractionSpawnDelays[distractionLevel - 1];
                    delay *= random.NextFloat(0.8f, 1.2f) * waitMultiplier;
                    nextScheduledDistractionTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(delay);
                }
            }
            
            foreach (IDistraction distraction in distractions)
            {
                if (distraction.Disposed)
                {
                    removalScheduled.Add(distraction);
                }
            }

            if (removalScheduled.Count > 0)
            {
                distractions.ExceptWith(removalScheduled);
                removalScheduled.Clear();
            }
            
            foreach (IDistraction distraction in distractions)
            {
                distraction.Update(gameTime, window.ClientBounds);
            }
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IDistraction distraction in distractions)
            {
                distraction.Draw(gameTime, spriteBatch, window.ClientBounds);
            }
        }

        public void AddDistraction(IDistraction distraction)
        {
            distraction.Scale *= scale;
            
            distraction.LoadContent(contentManager);
            distraction.Initialize();
            
            distractions.Add(distraction);
        }

        public void RemoveDistraction(IDistraction distraction)
        {
            removalScheduled.Add(distraction);
        }

        private void OnSetDistractionLevelMessage(SetDistractionLevelMessage message)
        {
            if(message.Level > MaxDistractionLevel && message.Level < 0) return;
            
            if (message.Level > 0)
            {
                if(message.Level > distractionLevel) nextScheduledDistractionTime = TimeSpan.MinValue;
            }
            else
            {
                foreach (IDistraction distraction in distractions)
                {
                    removalScheduled.Add(distraction);
                }
            }
            
            distractionLevel = message.Level;
        }
    }
}