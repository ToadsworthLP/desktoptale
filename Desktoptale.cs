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
        public static string ApplicationPath { get; private set; }
        public static string CustomCharacterPath { get; private set; }

        private Settings settings;
        private IRegistry<CharacterType, string> characterRegistry { get; }
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager;
        private PresetManager presetManager;
        private MonitorManager monitorManager;
        private WindowTracker windowTracker;
        private PartyManager partyManager;
        private ContextMenu contextMenu;
        private InteractionManager interactionManager;
        private Physics physics;
        private GlobalSettingsManager globalSettingsManager;
        private DistractionManager distractionManager;
        
        private ISet<ICharacter> characters;
        
        private const int WINDOW_STATE_UPDATE_INTERVAL = 20;
        private int windowStateUpdateCounter = 0;
        private bool firstFrame = true;
        private WindowInfo containingWindow;
        private Point defaultCharacterStartPosition;
        private Random rng = new Random();
        private bool globalPause = false;

        private readonly Color clearColor = new Color(0f, 0f, 0f, 0f);
        private const string globalSettingsFilePath = "GlobalSettings.yaml";

        private ConcurrentQueue<Action> UpdateTaskQueue;

        public Desktoptale(Settings settings)
        {
            ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            CustomCharacterPath = Path.Combine(ApplicationPath, "Content/Custom/");
            
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
            MessageBus.Subscribe<AddCharacterRequestedMessage>(OnAddCharacterRequestedMessage);
            MessageBus.Subscribe<GlobalPauseMessage>(OnGlobalPauseMessage);
            
            inputManager = new InputManager(monitorManager);
            partyManager = new PartyManager();
            presetManager = new PresetManager(characterRegistry, partyManager);
            physics = new Physics(inputManager);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            interactionManager = new InteractionManager(monitorManager, Window, inputManager);
            
            characters = new HashSet<ICharacter>();
            
            defaultCharacterStartPosition = monitorManager.ToMonoGameCoordinates(Window.ClientBounds.Center.ToVector2()).ToPoint();
            UpdateWindow();
            
            AddCharacterFromSettings(settings);
            
            globalSettingsManager = new GlobalSettingsManager(globalSettingsFilePath);
            if (!globalSettingsManager.DoesGlobalSettingsFileExist())
            {
                DisplayWelcomeMessage();
                globalSettingsManager.GlobalSettings = new GlobalSettings();
                globalSettingsManager.SaveGlobalSettings();
            }
            else
            {
                globalSettingsManager.LoadGlobalSettings();
            }
            
            distractionManager = new DistractionManager(Content, Window, windowTracker);
            distractionManager.Initialize();
            
            contextMenu = new ContextMenu(inputManager, characterRegistry, globalSettingsManager.GlobalSettings, partyManager);
            
            globalSettingsManager.SendMessages();
        }

        private void OnGlobalPauseMessage(GlobalPauseMessage message)
        {
            globalPause = message.Paused;

            if (message.Paused)
            {
                WindowsUtils.MakeClickthrough(Window);
            }
            else
            {
                WindowsUtils.MakeTopmostWindow(Window);
            }
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
            if (characterProperties != null && characterProperties.Position.X < 0 && characterProperties.Position.Y < 0)
                characterProperties.Position = defaultCharacterStartPosition.ToVector2();

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

                Party party = null;
                if (!string.IsNullOrWhiteSpace(settings.Party))
                {
                    party = partyManager.GetOrCreateParty(settings.Party);
                }

                characterProperties = new CharacterProperties(
                    initialCharacter,
                    defaultCharacterStartPosition.ToVector2(),
                    new Vector2(sourceSettings.Scale),
                    sourceSettings.IdleRoaming,
                    sourceSettings.UnfocusedInput,
                    stayInsideWindow,
                    party
                );
            }
            
            MessageBus.Send(new AddCharacterMessage() { Properties = characterProperties });
        }

        protected override void LoadContent()
        {
            ExternalCharacterFactory externalCharacterFactory = new ExternalCharacterFactory(CustomCharacterPath, graphics.GraphicsDevice);
            externalCharacterFactory.AddAllToRegistry(characterRegistry);
            
            if(settings.PrintRegistryKeys) PrintRegistryKeys();
        }
        
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (globalPause) return;
            
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
            
            distractionManager.Update(gameTime);
            
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
            
            distractionManager.Draw(gameTime, spriteBatch);
            
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
            
            character.Initialize(Character.CharacterCreationReason.NewCharacter);
            characters.Add(character);
            physics.AddCollider(character);
            
            MessageBus.Send(new FocusCharacterMessage() {Character = character });
        }

        private void OnRemoveCharacterMessage(RemoveCharacterMessage message)
        {
            physics.RemoveCollider(message.Target);
            characters.Remove(message.Target);
            message.Target.Dispose(Character.CharacterRemovalReason.RemoveCharacter);

            if (characters.Count == 0)
            {
                Exit();
            }
        }
        
        private void OnAddCharacterRequestedMessage(AddCharacterRequestedMessage message)
        {
            CharacterProperties characterProperties = new CharacterProperties(
                message.Character,
                message.Target.Properties.Position + new Vector2((float)((rng.NextDouble() - 0.5d) * message.Target.HitBox.Width), (message.Target.HitBox.Height / 2f)),
                message.Target.Properties.Scale,
                message.Target.Properties.IdleRoamingEnabled,
                message.Target.Properties.UnfocusedInputEnabled,
                message.Target.TrackedWindow?.Window,
                message.Target.Properties.Party
            );
            
            Character newCharacter;
            try
            {
                newCharacter = message.Character.FactoryFunction
                    .Invoke(new CharacterCreationContext(characterProperties, spriteBatch, inputManager, monitorManager, windowTracker));

                newCharacter.LoadContent(Content);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to switch character: {e.Message}");
                WindowsUtils.ShowMessageBox($"Failed to switch character: {e.Message}", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            newCharacter.Properties.Type = message.Character;
            newCharacter.Initialize(Character.CharacterCreationReason.NewCharacter);
            characters.Add(newCharacter);
            physics.AddCollider(newCharacter);
            
            MessageBus.Send(new FocusCharacterMessage() {Character = newCharacter });
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
            oldCharacter.Dispose(Character.CharacterRemovalReason.ChangeCharacter);

            newCharacter.Properties.Type = message.Character;
            newCharacter.Initialize(Character.CharacterCreationReason.ChangeCharacter);
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
