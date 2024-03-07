using System.Collections.Generic;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Desktoptale.Messaging;
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

        private CharacterFactory characterFactory;
        private Character character;
        private ISet<IGameObject> gameObjects;
        
        public Desktoptale()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = ProgramInfo.NAME;
            
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
            base.Initialize();
            
            MessageBus.Subscribe<CharacterChangeRequestedMessage>(OnCharacterChangeRequestedMessage);
            
            inputManager = new InputManager(this, GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            characterFactory = new CharacterFactory();
            
            gameObjects = new HashSet<IGameObject>();

            ContextMenu contextMenu = new ContextMenu(Window, inputManager, GraphicsDevice);
            contextMenu.Initialize();
            gameObjects.Add(contextMenu);
            
            MessageBus.Send(new CharacterChangeRequestedMessage { Character = CharacterType.Clover });
            MessageBus.Send(new ScaleChangeRequestedMessage { ScaleFactor = 2 });
            MessageBus.Send(new IdleMovementChangeRequestedMessage { Enabled = true });
            MessageBus.Send(new UnfocusedMovementChangeRequestedMessage { Enabled = false });
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
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
        
        private void OnCharacterChangeRequestedMessage(CharacterChangeRequestedMessage message)
        {
            Character newCharacter =
                characterFactory.Create(message.Character, graphics, Window, spriteBatch, inputManager);
            
            newCharacter.LoadContent(Content);

            if (character != null)
            {
                newCharacter.Position = character.Position;
                newCharacter.Scale = character.Scale;
                newCharacter.EnableIdleMovement = character.EnableIdleMovement;
                gameObjects.Remove(character);
                character.Dispose();
            }
            else
            {
                Point screenSize = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                newCharacter.Position = new Vector2(screenSize.X / 2.0f, screenSize.Y / 2.0f);
            }
            
            newCharacter.Initialize();
            gameObjects.Add(newCharacter);
            character = newCharacter;
        }
    }
}
