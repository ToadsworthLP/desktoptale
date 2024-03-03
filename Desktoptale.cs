using System.Collections.Generic;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Desktoptale
{
    public class Desktoptale : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager;

        private Character character;
        private ISet<IGameObject> gameObjects;
        
        public Desktoptale()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = "Clover";
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            WindowsUtils.MakeWindowOverlay(Window);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            inputManager = new InputManager(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            character = new Clover(graphics, Window, spriteBatch, inputManager);
            
            gameObjects = new HashSet<IGameObject>();
            gameObjects.Add(character);
            gameObjects.Add(new ContextMenu(Window, inputManager));
            
            base.Initialize();
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Initialize();
            }
            
            MessageBus.Send(new ScaleChangeRequestedMessage {ScaleFactor = 2});
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.LoadContent(Content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(gameTime);
            }

            WindowsUtils.MakeTopmostWindow(Window);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
