using System.Collections.Generic;
using System.Windows.Forms;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public class ContextMenu
    {
        private GameWindow window;
        private InputManager inputManager;
        private GraphicsDevice graphicsDevice;
        private IRegistry<CharacterType, string> characterRegistry;
        
        private int currentScaleFactor;
        private CharacterType currentCharacter;
        private bool idleMovementEnabled = true;
        private bool unfocusedMovementEnabled = false;
        private WindowInfo currentContainingWindow;
        
        private ContextMenuStrip currentContextMenuStrip = null;

        public ContextMenu(GameWindow window, InputManager inputManager, GraphicsDevice graphicsDevice, IRegistry<CharacterType, string> characterRegistry)
        {
            this.window = window;
            this.inputManager = inputManager;
            this.graphicsDevice = graphicsDevice;
            this.characterRegistry = characterRegistry;
            
            MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
            MessageBus.Subscribe<CharacterChangeSuccessMessage>(OnCharacterChangeSuccessMessage);
            MessageBus.Subscribe<IdleRoamingChangedMessage>(OnIdleMovementChangeRequestedMessage);
            MessageBus.Subscribe<UnfocusedMovementChangedMessage>(OnUnfocusedMovementChangeRequestedMessage);
            MessageBus.Subscribe<ChangeContainingWindowMessage>(OnChangeContainingWindowMessage);
            MessageBus.Subscribe<OpenContextMenuRequestedMessage>(OnOpenContextMenuRequested);
        }
        
        private void OnOpenContextMenuRequested(OpenContextMenuRequestedMessage message)
        {
            if (currentContextMenuStrip != null)
            {
                currentContextMenuStrip.Close();
            }

            OpenContextMenu(inputManager.VirtualScreenPointerPosition, message.Target);
        }

        private void OpenContextMenu(Point mousePosition, ICharacter target)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            
            ToolStripMenuItem characterItem = new ToolStripMenuItem("Character");
            contextMenuStrip.Items.Add(characterItem);

            SetupCharacterItems(characterItem, target);

            ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
            contextMenuStrip.Items.Add(scaleItem);

            SetupScaleOptions(scaleItem, target);
            
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            contextMenuStrip.Items.Add(settingsItem);
            
            SetupSettingsItems(settingsItem, target);
            
            ToolStripMenuItem savePresetItem = new ToolStripMenuItem("Save Preset...", null, (o, e) => { MessageBus.Send(new SavePresetRequestedMessage() { Target = target }); });
            contextMenuStrip.Items.Add(savePresetItem);
            
            ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove", null, (o, e) => { MessageBus.Send(new RemoveCharacterMessage() { Target = target }); });
            contextMenuStrip.Items.Add(removeItem);
            
            ToolStripMenuItem infoItem = new ToolStripMenuItem("About", null, (o, e) => ShowInfoScreen());
            contextMenuStrip.Items.Add(infoItem);

            contextMenuStrip.Closed += (o, e) =>
            {
                currentContextMenuStrip = null;
                MessageBus.Send(new ContextMenuStateChangedMessage() { Open = false });
            };
            
            contextMenuStrip.Opened += (o, e) =>
            {
                currentContextMenuStrip = contextMenuStrip;
                MessageBus.Send(new ContextMenuStateChangedMessage() { Open = true });
            };
            
            contextMenuStrip.Show(mousePosition.X, mousePosition.Y);
        }

        private void SetupSettingsItems(ToolStripMenuItem settingsItem, ICharacter target)
        {
            ToolStripMenuItem idleMovementItem = new ToolStripMenuItem($"Idle Roaming", null, (o, e) => MessageBus.Send(new IdleRoamingChangedMessage { Enabled = !idleMovementEnabled, Target = target}));
            idleMovementItem.Checked = idleMovementEnabled;
            settingsItem.DropDownItems.Add(idleMovementItem);
            
            ToolStripMenuItem unfocusedMovementItem = new ToolStripMenuItem($"Unfocused Input", null, (o, e) =>
            {
                MessageBus.Send(new UnfocusedMovementChangedMessage { Enabled = !unfocusedMovementEnabled });
            });
            unfocusedMovementItem.Checked = unfocusedMovementEnabled;
            settingsItem.DropDownItems.Add(unfocusedMovementItem);

            ToolStripMenuItem stayInWindowItem = new ToolStripMenuItem("Stay in Window", null, (o, e) => { });
            settingsItem.DropDownItems.Add(stayInWindowItem);
            SetupWindowSelectItems(stayInWindowItem, target);
            
            ToolStripMenuItem associatePresetItem = new ToolStripMenuItem("Associate Preset Files", null, (o, e) => { MessageBus.Send(new SetPresetFileAssociationRequestedMessage()); });
            settingsItem.DropDownItems.Add(associatePresetItem);
        }

        private void SetupCharacterItems(ToolStripMenuItem characterItem, ICharacter target)
        {
            IDictionary<string, ToolStripMenuItem> categoryItems = new Dictionary<string, ToolStripMenuItem>();

            foreach (CharacterType character in characterRegistry.GetAll())
            {
                ToolStripMenuItem characterSelectItem = new ToolStripMenuItem(character.Name, null, (o, e) => MessageBus.Send(new CharacterChangeRequestedMessage { Character = character, Target = target}));
                characterSelectItem.Checked = currentCharacter == character;
                
                ToolStripMenuItem parent;
                if (character.Category == null)
                {
                    parent = characterItem;
                }
                else
                {
                    if (!categoryItems.ContainsKey(character.Category))
                    {
                        ToolStripMenuItem categoryItem = new ToolStripMenuItem(character.Category);
                        characterItem.DropDownItems.Add(categoryItem);
                        categoryItems.Add(character.Category, categoryItem);
                    }

                    parent = categoryItems[character.Category];
                }

                parent.DropDownItems.Add(characterSelectItem);
            }
        }

        private void SetupScaleOptions(ToolStripMenuItem scaleItem, ICharacter target)
        {
            for (int i = 1; i < 9; i++)
            {
                var scaleFactor = i;
                ToolStripMenuItem scaleSelectItem = new ToolStripMenuItem($"{i}x", null, (o, e) => MessageBus.Send(new ScaleChangeRequestedMessage { ScaleFactor = scaleFactor, Target = target }));
                scaleSelectItem.Checked = currentScaleFactor == i;
                scaleItem.DropDownItems.Add(scaleSelectItem);
            }
        }

        private void SetupWindowSelectItems(ToolStripMenuItem parent, ICharacter target)
        {
            ToolStripMenuItem noneItem = new ToolStripMenuItem("None", null, (o, e) =>
            {
                MessageBus.Send(new ChangeContainingWindowMessage() { Window = null, Target = target });
            });
            noneItem.Checked = currentContainingWindow == null;
            parent.DropDownItems.Add(noneItem);
            
            IList<WindowInfo> windows = WindowsUtils.GetOpenWindows();
            foreach (WindowInfo windowInfo in windows)
            {
                string title;
                if (windowInfo.Title.Length > 50)
                {
                    title = $"{windowInfo.Title.Substring(0, 47)}... [{windowInfo.ProcessName}]";
                }
                else
                {
                    title = $"{windowInfo.Title} [{windowInfo.ProcessName}]";
                }
                
                ToolStripMenuItem item = new ToolStripMenuItem(title, null, (o, e) =>
                {
                    MessageBus.Send(new ChangeContainingWindowMessage() { Window = windowInfo, Target = target });
                });
                item.Checked = currentContainingWindow != null && currentContainingWindow.hWnd == windowInfo.hWnd;
                parent.DropDownItems.Add(item);
            }
        }
        
        private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
        {
            currentScaleFactor = (int)message.ScaleFactor;
        }

        private void OnCharacterChangeSuccessMessage(CharacterChangeSuccessMessage message)
        {
            currentCharacter = message.Character;
        }
        
        private void OnIdleMovementChangeRequestedMessage(IdleRoamingChangedMessage message)
        {
            idleMovementEnabled = message.Enabled;
        }
        
        private void OnUnfocusedMovementChangeRequestedMessage(UnfocusedMovementChangedMessage message)
        {
            unfocusedMovementEnabled = message.Enabled;
        }
        
        private void OnChangeContainingWindowMessage(ChangeContainingWindowMessage message)
        {
            currentContainingWindow = message.Window;
        }

        private void ShowInfoScreen()
        {
            WindowsUtils.ShowMessageBox($"{ProgramInfo.NAME} {ProgramInfo.VERSION}\nCreated by {ProgramInfo.AUTHOR}\n\n{ProgramInfo.CREDITS}\n{ProgramInfo.DISCLAIMER}", "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}