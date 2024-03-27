using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
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
        private Settings settings;
        private IRegistry<CharacterType, string> characterRegistry { get; }
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager;
        private PresetManager presetManager;
        private MonitorManager monitorManager;
        
        private Character character;
        private ISet<IGameObject> gameObjects;
        
        private const int WINDOW_STATE_UPDATE_INTERVAL_MIN = 28;
        private const int WINDOW_STATE_UPDATE_INTERVAL_MAX = 30;
        private int windowStateUpdateCounter = 0;
        private int nextWindowStateUpdate = 0;
        private bool firstFrame = true;
        private bool alwaysOnTop;
        private WindowsUtils.WindowInfo containingWindow;
        private string applicationPath;

        private Random rng;

        private CharacterSpriteEffect characterSpriteEffect;
        private readonly Color clearColor = new Color(0f, 0f, 0f, 0f);

        public Desktoptale(Settings settings)
        {
            applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            this.settings = settings;
            this.rng = new Random();
            
            graphics = new GraphicsDeviceManager(this);
            Window.Title = ProgramInfo.NAME;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            characterRegistry = new CharacterRegistry();
            
            monitorManager = new MonitorManager();
            
            WindowsUtils.PrepareWindow(Window);
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            
            MessageBus.Subscribe<CharacterChangeRequestedMessage>(OnCharacterChangeRequestedMessage);
            MessageBus.Subscribe<ChangeContainingWindowMessage>(OnChangeContainingWindowMessage);
            MessageBus.Subscribe<AlwaysOnTopChangeRequestedMessage>(OnAlwaysOnTopChangeRequestedMessage);
            
            // Keep settings object up-to-date
            MessageBus.Subscribe<CharacterChangeSuccessMessage>(msg => settings.Character = msg.Character.ToString());
            MessageBus.Subscribe<ScaleChangeRequestedMessage>(msg => settings.Scale = (int)msg.ScaleFactor);
            MessageBus.Subscribe<IdleMovementChangeRequestedMessage>(msg => settings.IdleRoaming = msg.Enabled);
            MessageBus.Subscribe<UnfocusedMovementChangeRequestedMessage>(msg => settings.UnfocusedInput = msg.Enabled);
            MessageBus.Subscribe<AlwaysOnTopChangeRequestedMessage>(msg => settings.AlwaysOnTop = msg.Enabled);
            MessageBus.Subscribe<ChangeContainingWindowMessage>(msg => settings.Window = msg.Window?.ProcessName);
            
            // Preset loading
            presetManager = new PresetManager(settings);
            presetManager.LoadPreset();
            
            inputManager = new InputManager(this, GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            gameObjects = new HashSet<IGameObject>();

            ContextMenu contextMenu = new ContextMenu(Window, inputManager, GraphicsDevice, characterRegistry);
            contextMenu.Initialize();
            gameObjects.Add(contextMenu);
            
            FirstStartCheck();

            // Send initialization messages
            CharacterType initialCharacter = CharacterRegistry.FRISK;
            if (settings.Character != null)
            {
                try
                {
                    initialCharacter = characterRegistry.Get(settings.Character);
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine($"Failed to switch character: Invalid character registry key: {settings.Character}");
                    WindowsUtils.ShowMessageBox($"Could not find character: {settings.Character}\nIf this character is a custom character, please make sure that it is installed properly.", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            
            MessageBus.Send(new CharacterChangeRequestedMessage { Character = initialCharacter });
            MessageBus.Send(new ScaleChangeRequestedMessage { ScaleFactor = settings.Scale });
            MessageBus.Send(new IdleMovementChangeRequestedMessage { Enabled = settings.IdleRoaming });
            MessageBus.Send(new AlwaysOnTopChangeRequestedMessage() { Enabled = settings.AlwaysOnTop });
            MessageBus.Send(new UnfocusedMovementChangeRequestedMessage { Enabled = settings.UnfocusedInput });

            if (!string.IsNullOrWhiteSpace(settings.Window))
            {
                WindowsUtils.WindowInfo target = WindowsUtils.GetWindowByName(settings.Window);
                if (target != null)
                {
                    MessageBus.Send(new ChangeContainingWindowMessage() { Window = target });
                }
                else
                {
                    Console.WriteLine($"Failed to attach to window: Window or process {settings.Window} could not be found.");
                }
            }
        }
        
        protected override void LoadContent()
        {
            characterSpriteEffect = new CharacterSpriteEffect(GraphicsDevice);
            
            ExternalCharacterFactory externalCharacterFactory = new ExternalCharacterFactory(Path.Combine(applicationPath, "Content/Custom/"), graphics.GraphicsDevice);
            externalCharacterFactory.AddAllToRegistry(characterRegistry);
            
            if(settings.PrintRegistryKeys) PrintRegistryKeys();
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (firstFrame)
            {
                WindowsUtils.PrepareWindow(Window);
                WindowsUtils.MakeTopmostWindow(Window);

                firstFrame = false;
            }
            
            inputManager.Update();
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(gameTime);
            }

            if (windowStateUpdateCounter >= nextWindowStateUpdate)
            {
                if(alwaysOnTop) WindowsUtils.MakeTopmostWindow(Window);
                if(containingWindow != null) UpdateBounds();
                
                windowStateUpdateCounter = 0;
                nextWindowStateUpdate = rng.Next(WINDOW_STATE_UPDATE_INTERVAL_MIN, WINDOW_STATE_UPDATE_INTERVAL_MAX + 1);
            }

            windowStateUpdateCounter++;
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(clearColor);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, effect: characterSpriteEffect);
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private void OnCharacterChangeRequestedMessage(CharacterChangeRequestedMessage message)
        {
            Character newCharacter;
            try
            {
                newCharacter = message.Character.FactoryFunction
                    .Invoke(new CharacterCreationContext(graphics, Window, spriteBatch, inputManager, monitorManager));

                newCharacter.LoadContent(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to switch character: {e.Message}");
                WindowsUtils.ShowMessageBox($"Failed to switch character: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBus.Send(new UpdateBoundaryMessage() {  Boundary = null });
            }
        }

        private void OnAlwaysOnTopChangeRequestedMessage(AlwaysOnTopChangeRequestedMessage message)
        {
            alwaysOnTop = message.Enabled;
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

        private void PrintRegistryKeys()
        {
            Console.WriteLine("Currently registered characters:");
            foreach (string key in characterRegistry.GetAllIds())
            {
                Console.WriteLine(key);
            }
        }

        private void FirstStartCheck()
        {
            string path = Path.Combine(applicationPath, ".desktoptale");
            if (!File.Exists(path))
            {
                DisplayWelcomeMessage();
                try
                {
                    File.Create(path).Dispose();
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                }
                catch (Exception e) {}
            }
        }

        private void DisplayWelcomeMessage()
        {
            DialogResult result = WindowsUtils.ShowMessageBox(ProgramInfo.WELCOME_MESSAGE, ProgramInfo.NAME,
                MessageBoxButtons.YesNo, MessageBoxIcon.None);

            if (result == DialogResult.Yes)
            {
                MessageBus.Send(new SetPresetFileAssociationRequestedMessage());
            }
        }
    }
}
