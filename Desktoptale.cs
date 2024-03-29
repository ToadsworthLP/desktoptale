using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CommandLine;
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
        private WindowTracker windowTracker;
        private ContextMenu contextMenu;
        private Physics physics;
        
        private ISet<ICharacter> characters;
        
        private const int WINDOW_STATE_UPDATE_INTERVAL = 20;
        private int windowStateUpdateCounter = 0;
        private bool firstFrame = true;
        private WindowInfo containingWindow;
        private string applicationPath;
        private Point defaultCharacterStartPosition;

        private readonly Color clearColor = new Color(0f, 0f, 0f, 0f);

        private ConcurrentQueue<Action> UpdateTaskQueue;

        public Desktoptale(Settings settings)
        {
            applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            this.settings = settings;
            
            graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false,
                IsFullScreen = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            Window.Title = ProgramInfo.NAME;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            characterRegistry = new CharacterRegistry();
            monitorManager = new MonitorManager();
            windowTracker = new WindowTracker(monitorManager);
            
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d/60d);
            
            UpdateTaskQueue = new ConcurrentQueue<Action>();
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            MessageBus.Subscribe<OtherInstanceStartedMessage>(OnOtherInstanceStartedMessage);
            MessageBus.Subscribe<AddCharacterMessage>(OnAddCharacterMessage);
            MessageBus.Subscribe<RemoveCharacterMessage>(OnRemoveCharacterMessage);
            MessageBus.Subscribe<CharacterChangeRequestedMessage>(OnCharacterChangeRequestedMessage);
            MessageBus.Subscribe<DisplaySettingsChangedMessage>(OnDisplaySettingsChangedMessage);
            
            // Keep settings object up-to-date
            // MessageBus.Subscribe<CharacterChangeSuccessMessage>(msg => settings.Character = msg.Character.ToString());
            // MessageBus.Subscribe<ScaleChangeRequestedMessage>(msg => settings.Scale = (int)msg.ScaleFactor);
            // MessageBus.Subscribe<IdleRoamingChangedMessage>(msg => settings.IdleRoaming = msg.Enabled);
            // MessageBus.Subscribe<UnfocusedMovementChangedMessage>(msg => settings.UnfocusedInput = msg.Enabled);
            // MessageBus.Subscribe<ChangeContainingWindowMessage>(msg => settings.Window = msg.Window?.ProcessName);

            inputManager = new InputManager(this, GraphicsDevice, monitorManager);
            presetManager = new PresetManager(characterRegistry);
            physics = new Physics(inputManager);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            contextMenu = new ContextMenu(inputManager, characterRegistry);
            
            characters = new HashSet<ICharacter>();
            
            FirstStartCheck();

            defaultCharacterStartPosition = monitorManager.ToMonoGameCoordinates(Window.ClientBounds.Center.ToVector2()).ToPoint();
            UpdateWindow();
            
            AddCharacterFromSettings(settings);
        }
        
        private void OnOtherInstanceStartedMessage(OtherInstanceStartedMessage message)
        {
            Settings settings = new Settings();
            if (message.Args != null && message.Args.Length > 0)
            {
                Parser parser = new Parser(config =>
                {
                    config.AutoHelp = false;
                    config.AutoVersion = false;
                });
                parser.ParseArguments<Settings>(message.Args)
                    .WithParsed<Settings>((s) =>
                    {
                        settings = s;
                    });
            }
            
            UpdateTaskQueue.Enqueue(() => AddCharacterFromSettings(settings));
        }

        private void AddCharacterFromSettings(Settings sourceSettings)
        {
            // Preset loading
            CharacterProperties characterProperties = presetManager.LoadPreset(sourceSettings.Preset);

            // If no preset had been loaded, create a character according to the CLI settings
            if (characterProperties == null)
            {
                CharacterType initialCharacter = CharacterRegistry.FRISK;
                if (!string.IsNullOrWhiteSpace(sourceSettings.Character))
                {
                    try
                    {
                        initialCharacter = characterRegistry.Get(sourceSettings.Character);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine($"Failed to add character: Invalid character registry key: {sourceSettings.Character}");
                        WindowsUtils.ShowMessageBox($"Could not find character: {sourceSettings.Character}\nIf this character is a custom character, please make sure that it is installed properly.", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                WindowInfo stayInsideWindow = null;
                if (!string.IsNullOrWhiteSpace(sourceSettings.Window))
                {
                    WindowInfo target = WindowsUtils.GetWindowByName(sourceSettings.Window);
                    if (target != null)
                    {
                        stayInsideWindow = target;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to attach to window: Window or process {sourceSettings.Window} could not be found.");
                    }
                }

                characterProperties = new CharacterProperties(
                    initialCharacter,
                    defaultCharacterStartPosition.ToVector2(),
                    new Vector2(sourceSettings.Scale),
                    sourceSettings.IdleRoaming,
                    sourceSettings.UnfocusedInput,
                    stayInsideWindow
                );
            }
            
            MessageBus.Send(new AddCharacterMessage() { Properties = characterProperties });
        }

        protected override void LoadContent()
        {
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
                UpdateWindow();
            
                firstFrame = false;
            }

            while (!UpdateTaskQueue.IsEmpty)
            {
                Action action = null;
                UpdateTaskQueue.TryDequeue(out action);
                action?.Invoke();
            }
            
            inputManager.Update();
            windowTracker.Update();
            
            foreach (var gameObject in characters)
            {
                gameObject.Update(gameTime);
            }
            
            physics.Update();
            if (physics.HasColliderUnderCursorChanged)
            {
                if (physics.PhysicsObjectUnderCursor == null)
                {
                    WindowsUtils.MakeClickthrough(Window);
                }
                else
                {
                    WindowsUtils.MakeClickable(Window);
                }
            }
            
            if (windowStateUpdateCounter >= WINDOW_STATE_UPDATE_INTERVAL)
            {
                WindowsUtils.MakeTopmostWindow(Window);
                windowStateUpdateCounter = 0;
            }

            windowStateUpdateCounter++;
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(clearColor);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            foreach (var gameObject in characters)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateWindow()
        {
            Rectangle boundingRect = monitorManager.BoundingRectangle;
            
            graphics.PreferredBackBufferWidth = boundingRect.Width;
            graphics.PreferredBackBufferHeight = boundingRect.Height;
            graphics.ApplyChanges();
            
            Window.Position = boundingRect.Location;
        }

        private void OnDisplaySettingsChangedMessage(DisplaySettingsChangedMessage message)
        {
            UpdateWindow();
        }

        private void OnAddCharacterMessage(AddCharacterMessage message)
        {
            Character character;
            try
            {
                character = message.Properties.Type.FactoryFunction
                    .Invoke(new CharacterCreationContext(message.Properties, spriteBatch, inputManager, monitorManager, windowTracker));

                character.LoadContent(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to add character: {e.Message}");
                WindowsUtils.ShowMessageBox($"Failed to add character: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            character.Initialize();
            characters.Add(character);
            physics.AddCollider(character);
            
            MessageBus.Send(new FocusCharacterMessage() {Character = character });
        }

        private void OnRemoveCharacterMessage(RemoveCharacterMessage message)
        {
            physics.RemoveCollider(message.Target);
            characters.Remove(message.Target);
            message.Target.Dispose();

            if (characters.Count == 0)
            {
                Exit();
            }
        }
        
        private void OnCharacterChangeRequestedMessage(CharacterChangeRequestedMessage message)
        {
            ICharacter oldCharacter = message.Target;
            Character newCharacter;
            try
            {
                newCharacter = message.Character.FactoryFunction
                    .Invoke(new CharacterCreationContext(new CharacterProperties(oldCharacter.Properties), spriteBatch, inputManager, monitorManager, windowTracker));

                newCharacter.LoadContent(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to switch character: {e.Message}");
                WindowsUtils.ShowMessageBox($"Failed to switch character: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            
            physics.RemoveCollider(oldCharacter);
            characters.Remove(oldCharacter);
            oldCharacter.Dispose();

            newCharacter.Properties.Type = message.Character;
            newCharacter.Initialize();
            characters.Add(newCharacter);
            physics.AddCollider(newCharacter);
            
            MessageBus.Send(new CharacterChangeSuccessMessage { Character = message.Character });
            MessageBus.Send(new FocusCharacterMessage() {Character = newCharacter });
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
