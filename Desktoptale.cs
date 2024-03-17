using System;
using System.Collections.Generic;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Desktoptale
{
    public class Desktoptale : Game
    {
        private IRegistry<CharacterType, int> characterRegistry { get; }
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager;

        private Character character;
        private ISet<IGameObject> gameObjects;
        
        private const int WINDOW_STATE_UPDATE_INTERVAL = 20;
        private int windowStateUpdateCounter = 0;
        private bool enableRepeatedForceTopmost = false;
        private bool firstFrame = true;
        private WindowsUtils.WindowInfo containingWindow;

        public Desktoptale()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = ProgramInfo.NAME;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            characterRegistry = new CharacterRegistry();
            
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
            MessageBus.Subscribe<UnfocusedMovementChangeRequestedMessage>(OnUnfocusedMovementChangeRequestedMessage);
            MessageBus.Subscribe<ChangeContainingWindowMessage>(OnChangeContainingWindowMessage);
            
            inputManager = new InputManager(this, GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            gameObjects = new HashSet<IGameObject>();

            ContextMenu contextMenu = new ContextMenu(Window, inputManager, GraphicsDevice, characterRegistry);
            contextMenu.Initialize();
            gameObjects.Add(contextMenu);
            
            MessageBus.Send(new CharacterChangeRequestedMessage { Character = CharacterRegistry.FRISK});
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
            ExternalCharacterFactory externalCharacterFactory = new ExternalCharacterFactory("Content/Custom/", graphics.GraphicsDevice);
            externalCharacterFactory.AddAllToRegistry(characterRegistry);
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
            if (firstFrame)
            {
                WindowsUtils.MakeTopmostWindow(Window);

                firstFrame = false;
            }
            
            inputManager.Update();
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(gameTime);
            }

            if (windowStateUpdateCounter % WINDOW_STATE_UPDATE_INTERVAL == 0)
            {
                if(enableRepeatedForceTopmost) WindowsUtils.MakeTopmostWindow(Window);
                if(containingWindow != null) UpdateBounds();
                
                windowStateUpdateCounter = 1;
            }

            windowStateUpdateCounter++;
            
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
            Character newCharacter;
            try
            {
                newCharacter = message.Character.FactoryFunction
                    .Invoke(new CharacterCreationContext(graphics, Window, spriteBatch, inputManager));

                newCharacter.LoadContent(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to switch character: {e.Message}");
                return;
            }

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
            
            MessageBus.Send(new CharacterChangeSuccessMessage { Character = message.Character });
        }

        private void OnUnfocusedMovementChangeRequestedMessage(UnfocusedMovementChangeRequestedMessage message)
        {
            if (message.Enabled)
            {
                enableRepeatedForceTopmost = true;
                windowStateUpdateCounter = 0;
            }
            else
            {
                enableRepeatedForceTopmost = false;
            }
        }

        private void OnChangeContainingWindowMessage(ChangeContainingWindowMessage message)
        {
            containingWindow = message.Window;
            if (message.Window != null)
            {
                Rectangle bounds = WindowsUtils.GetWindowRect(message.Window.hWnd);
                MessageBus.Send(new UpdateBoundaryMessage() { Boundary = bounds });
            }
            else
            {
                MessageBus.Send(new UpdateBoundaryMessage() { 
                    Boundary = new Rectangle(
                        Point.Zero, 
                        new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)) 
                    }
                );
            }
        }

        private void UpdateBounds()
        {
            Rectangle bounds = WindowsUtils.GetWindowRect(containingWindow.hWnd);

            if (bounds.IsEmpty)
            {
                MessageBus.Send(new ChangeContainingWindowMessage() { Window = null });
            }
            else
            {
                MessageBus.Send(new UpdateBoundaryMessage() { Boundary = bounds });
            }
        }
    }
}
