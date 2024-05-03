using System;
using System.Collections.Generic;
using Desktoptale.Distractions;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale
{
    public class DistractionManager : IDistractionManager
    {
        public const int MaxDistractionLevel = 5;
            
        private const float ScaleDivisor = 600f;

        private static readonly float[] DistractionSpawnDelays = new[] { 128f, 32f, 8f, 2f, 0.5f };
        
        private ContentManager contentManager;
        private GameWindow window;

        private IList<IDistractionPattern> patterns;
        private ISet<IDistraction> distractions;
        private Vector2 scale;

        private IList<IDistraction> removalScheduled;

        private int distractionLevel;
        private Random random;
        private int lastDistractionIndex = -1;

        private WindowTracker windowTracker;
        private TrackedWindow trackedWindow;

        private TimeSpan nextScheduledDistractionTime = TimeSpan.MinValue;
        
        public DistractionManager(ContentManager contentManager, GameWindow window, WindowTracker windowTracker)
        {
            this.contentManager = contentManager;
            this.window = window;
            this.windowTracker = windowTracker;

            patterns = new List<IDistractionPattern>();
            distractions = new HashSet<IDistraction>();
            removalScheduled = new List<IDistraction>();

            random = new Random();

            MessageBus.Subscribe<SetDistractionLevelMessage>(OnSetDistractionLevelMessage);
            MessageBus.Subscribe<SetDistractionScaleMessage>(OnSetDistractionScaleMessage);
            MessageBus.Subscribe<SetDistractionTrackedWindowMessage>(OnSetDistractionTrackedWindowMessage);
        }
        
        public void Initialize()
        {
            scale = new Vector2(2, 2);

            patterns.Add(new UpDownOppositeBonesPattern(6, 300f, 120f, 100));
            patterns.Add(new LeftRightOppositeBonesPattern(6, 300f, 120f, 100));
            patterns.Add(new ScreenEdgeBonesPattern(6, 250f, 120f, true));
            patterns.Add(new ScreenEdgeBonesPattern(6, 250f, 120f, false));
            patterns.Add(new RandomGasterBlasterPattern(5, 0.5f));
            patterns.Add(new SideGasterBlasterPattern(10, 0.25f));
        }
        
        public void Update(GameTime gameTime)
        {
            Rectangle bounds;
            if (trackedWindow?.Bounds != null)
            {
                bounds = trackedWindow.Bounds;
            }
            else
            {
                bounds = window.ClientBounds;
                if (bounds.X < 0) bounds.X = 0;
                if (bounds.Y < 0) bounds.Y = 0;
            }

            if (distractionLevel > 0)
            {
                if (nextScheduledDistractionTime < gameTime.TotalGameTime)
                {
                    int index = random.Next(0, patterns.Count);
                    if (index == lastDistractionIndex) index = (index + 1) % patterns.Count;
                    lastDistractionIndex = index;

                    
                    
                    float waitMultiplier = patterns[index].Spawn(this, bounds, scale);
                    
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
                distraction.Update(gameTime, bounds);
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

        public void RemoveAllDistractions()
        {
            foreach (IDistraction distraction in distractions)
            {
                removalScheduled.Add(distraction);
            }
        }

        private void OnSetDistractionLevelMessage(SetDistractionLevelMessage message)
        {
            if(message.Level > MaxDistractionLevel && message.Level < 0) return;

            if (message.Level != distractionLevel)
            {
                RemoveAllDistractions();
                nextScheduledDistractionTime = TimeSpan.MinValue;
            }
            
            distractionLevel = message.Level;
        }

        private void OnSetDistractionScaleMessage(SetDistractionScaleMessage message)
        {
            scale = new Vector2(message.Scale, message.Scale);
            RemoveAllDistractions();
            nextScheduledDistractionTime = TimeSpan.MinValue;
        }

        private void OnSetDistractionTrackedWindowMessage(SetDistractionTrackedWindowMessage message)
        {
            if ((trackedWindow != null && message.Window != null && trackedWindow.Window.hWnd != message.Window.hWnd) ||
                (trackedWindow == null && message.Window != null) ||
                (trackedWindow != null && message.Window == null))
            {
                RemoveAllDistractions();
                nextScheduledDistractionTime = TimeSpan.MinValue;
            }
            
            if (message.Window == null)
            {
                if (trackedWindow != null)
                {
                    windowTracker.Unsubscribe(trackedWindow.Window);
                    trackedWindow.WindowDestroyed -= OnContainingWindowDestroyed;
                    trackedWindow = null;
                }
            }
            else
            {
                if (trackedWindow != null)
                {
                    windowTracker.Unsubscribe(trackedWindow.Window);
                    trackedWindow.WindowDestroyed -= OnContainingWindowDestroyed;
                }
                
                trackedWindow = windowTracker.Subscribe(message.Window);
                trackedWindow.WindowDestroyed += OnContainingWindowDestroyed;
            }
        }
        
        private void OnContainingWindowDestroyed()
        {
            MessageBus.Send(new SetDistractionTrackedWindowMessage() { Window = null });
        }
    }
}