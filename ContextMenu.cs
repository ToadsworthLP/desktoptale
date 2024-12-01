using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Desktoptale.Characters;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.Registry;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class ContextMenu
    {
        private InputManager inputManager;
        private PartyManager partyManager;
        private IRegistry<CharacterType, string> characterRegistry;
        private ContextMenuStrip currentContextMenuStrip = null;

        private GlobalSettings globalSettings;
        private IntPtr distractionWindowHwnd;
        
        private static readonly string[] distractionLevelNames = new[] { "Off", "Low", "Medium", "High", "Very High", "Extreme" };

        public ContextMenu(InputManager inputManager, IRegistry<CharacterType, string> characterRegistry, GlobalSettings globalSettings, PartyManager partyManager)
        {
            this.inputManager = inputManager;
            this.characterRegistry = characterRegistry;
            this.globalSettings = globalSettings;
            this.partyManager = partyManager;
            
            MessageBus.Subscribe<OpenContextMenuRequestedMessage>(OnOpenContextMenuRequested);
            MessageBus.Subscribe<ClickThroughChangeRequestedMessage>(OnClickThroughChangeRequestedMessage);
            MessageBus.Subscribe<SetDistractionTrackedWindowMessage>(OnSetDistractionTrackedWindowMessage);
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
            
            ToolStripMenuItem switchCharacterItem = new ToolStripMenuItem("Character");
            contextMenuStrip.Items.Add(switchCharacterItem);

            SetupSwitchCharacterItems(switchCharacterItem, target);

            ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
            contextMenuStrip.Items.Add(scaleItem);

            SetupScaleOptions(scaleItem, target);
            
            ToolStripMenuItem partyItem = new ToolStripMenuItem("Party");
            contextMenuStrip.Items.Add(partyItem);

            SetupPartySettingsItems(partyItem, target);
            
            ToolStripMenuItem behaviorSettingsItem = new ToolStripMenuItem("Behavior");
            contextMenuStrip.Items.Add(behaviorSettingsItem);
            
            SetupBehaviorSettingsItems(behaviorSettingsItem, target);
            
            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Duplicate...", null, (o, e) =>
            {
                WindowsUtils.ShowDuplicateOptionsForm((count) =>
                {
                    DuplicateCharacter(target, count);
                }, () => {});
            });
            contextMenuStrip.Items.Add(duplicateItem);
            
            ToolStripMenuItem savePresetItem = new ToolStripMenuItem("Save Preset...", null, (o, e) => { MessageBus.Send(new SavePresetRequestedMessage() { Target = target }); });
            contextMenuStrip.Items.Add(savePresetItem);
            
            ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove", null, (o, e) => { MessageBus.Send(new RemoveCharacterMessage() { Target = target }); });
            contextMenuStrip.Items.Add(removeItem);

            contextMenuStrip.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem addCharacterItem = new ToolStripMenuItem("Add New");
            contextMenuStrip.Items.Add(addCharacterItem);
            
            SetupAddCharacterItems(addCharacterItem, target);
            
            ToolStripMenuItem distractionsItem = new ToolStripMenuItem("Distractions");
            contextMenuStrip.Items.Add(distractionsItem);
            
            for (int i = 0; i <= DistractionManager.MaxDistractionLevel; i++)
            {
                int level = i;
                
                ToolStripMenuItem distractionLevelItem = new ToolStripMenuItem(distractionLevelNames[level], null, (o, e) => { MessageBus.Send(new SetDistractionLevelMessage() { Level = level }); });
                distractionLevelItem.Checked = i == globalSettings.DistractionLevel;
                distractionsItem.DropDownItems.Add(distractionLevelItem);
            }
            
            distractionsItem.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem distractionsScaleItem = new ToolStripMenuItem("Scale");
            distractionsItem.DropDownItems.Add(distractionsScaleItem);
            
            SetupDistractionScaleOptions(distractionsScaleItem);
            
            ToolStripMenuItem distractionsWindowItem = new ToolStripMenuItem("Focus on Window");
            distractionsItem.DropDownItems.Add(distractionsWindowItem);
            
            SetupDistractionWindowSelectItems(distractionsWindowItem);
            
            ToolStripMenuItem globalSettingsItem = new ToolStripMenuItem("Settings");
            contextMenuStrip.Items.Add(globalSettingsItem);
            
            SetupGlobalSettingsItems(globalSettingsItem, target);
            
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

        private void SetupBehaviorSettingsItems(ToolStripMenuItem settingsItem, ICharacter target)
        {
            ToolStripMenuItem idleMovementItem = new ToolStripMenuItem($"Idle Roaming", null, (o, e) => MessageBus.Send(new IdleRoamingChangedMessage { Enabled = !target.Properties.IdleRoamingEnabled, Target = target}));
            idleMovementItem.Checked = target.Properties.IdleRoamingEnabled;
            settingsItem.DropDownItems.Add(idleMovementItem);
            
            ToolStripMenuItem unfocusedMovementItem = new ToolStripMenuItem($"Unfocused Input", null, (o, e) =>
            {
                MessageBus.Send(new UnfocusedMovementChangedMessage { Enabled = !target.Properties.UnfocusedInputEnabled, Target = target});
            });
            unfocusedMovementItem.Checked = target.Properties.UnfocusedInputEnabled;
            settingsItem.DropDownItems.Add(unfocusedMovementItem);

            ToolStripMenuItem stayInWindowItem = new ToolStripMenuItem("Stay in Window", null, (o, e) => { });
            settingsItem.DropDownItems.Add(stayInWindowItem);
            SetupWindowSelectItems(stayInWindowItem, target);
        }
        
        private void SetupPartySettingsItems(ToolStripMenuItem partyItem, ICharacter target)
        {
            ToolStripMenuItem noneItem = new ToolStripMenuItem("None", null, (o, e) =>
            {
                MessageBus.Send(new LeavePartyMessage() { Character = target, Party = target.Properties.Party });
            });

            noneItem.Checked = target.Properties.Party == null;
            partyItem.DropDownItems.Add(noneItem);
            
            foreach (Party party in partyManager.GetAllParties())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(party.Name, null, (o, e) =>
                {
                    MessageBus.Send(new LeavePartyMessage() { Character = target, Party = target.Properties.Party });
                    MessageBus.Send(new JoinPartyMessage() { Character = target, Party = party });
                });

                item.Checked = target.Properties.Party == party;
                partyItem.DropDownItems.Add(item);
            }
            
            ToolStripMenuItem createPartyItem = new ToolStripMenuItem("Create New...", null, (o, e) =>
            {
                WindowsUtils.ShowCreatePartyForm(name =>
                {
                    if (partyManager.IsNewPartyNameValid(name))
                    {
                        Party newParty = partyManager.GetOrCreateParty(name);
                        MessageBus.Send(new LeavePartyMessage() { Character = target, Party = target.Properties.Party });
                        MessageBus.Send(new JoinPartyMessage() { Character = target, Party = newParty });
                    }
                    else
                    {
                        WindowsUtils.ShowMessageBox("Party names have to be unique and cannot consist of only white space.", ProgramInfo.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }, () => { });
            });
            
            partyItem.DropDownItems.Add(createPartyItem);
        }
        
        private void SetupGlobalSettingsItems(ToolStripMenuItem settingsItem, ICharacter target)
        {
            ToolStripMenuItem clickThroughItem = new ToolStripMenuItem("Make Click-Through", null, (o, e) => { MessageBus.Send(new ClickThroughChangeRequestedMessage() { Enabled = !globalSettings.ClickThroughMode }); });
            clickThroughItem.Checked = globalSettings.ClickThroughMode;
            settingsItem.DropDownItems.Add(clickThroughItem);
            
            ToolStripMenuItem interactionButtonItem = new ToolStripMenuItem("Interaction Mouse Clicks", null, (o, e) => { MessageBus.Send(new InteractionButtonChangedMessage() { Enabled = !globalSettings.EnableInteractionButton }); });
            interactionButtonItem.Checked = globalSettings.EnableInteractionButton;
            settingsItem.DropDownItems.Add(interactionButtonItem);
            
            ToolStripMenuItem openCustomCharacterFolderItem = new ToolStripMenuItem("Open Custom Character Folder", null,
                (o, e) =>
                {
                    Process.Start(new ProcessStartInfo(Path.GetFullPath( Desktoptale.CustomCharacterPath )) { UseShellExecute = true });
                });
            settingsItem.DropDownItems.Add(openCustomCharacterFolderItem);
            
            ToolStripMenuItem associatePresetItem = new ToolStripMenuItem("Associate Preset Files", null, (o, e) => { MessageBus.Send(new SetPresetFileAssociationRequestedMessage()); });
            settingsItem.DropDownItems.Add(associatePresetItem);
        }

        private void SetupSwitchCharacterItems(ToolStripMenuItem characterItem, ICharacter target)
        {
            IDictionary<string, ToolStripMenuItem> categoryItems = new Dictionary<string, ToolStripMenuItem>();

            IEnumerable<CharacterType> displayedCharacterTypes = GetDisplayedCharacterTypes(characterRegistry.GetAll());
            foreach (CharacterType character in displayedCharacterTypes)
            {
                ToolStripMenuItem characterSelectItem = new ToolStripMenuItem(character.Name, null, (o, e) => MessageBus.Send(new CharacterChangeRequestedMessage { Character = character, Target = target}));
                characterSelectItem.Checked = target.Properties.Type == character;
                
                ToolStripMenuItem parent;
                if (character.Category == null)
                {
                    parent = characterItem;
                }
                else
                {
                    if (!categoryItems.ContainsKey(character.Category))
                    {
                        void MakeCategoriesRecursive(string[] splitCategory, int depth)
                        {
                            if (depth >= splitCategory.Length) return;
                            
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < depth; i++)
                            {
                                sb.Append(splitCategory[i]);
                                sb.Append("\\");
                            }
                            
                            string catString = depth > 0 ? sb.ToString(0, sb.Length - 1) : "";
                            string catStringWithCurrent = depth > 0 ? $"{catString}\\{splitCategory[depth]}" : splitCategory[depth];

                            if (!categoryItems.ContainsKey(catStringWithCurrent))
                            {
                                ToolStripMenuItem categoryItem = new ToolStripMenuItem(splitCategory[depth]);
                                categoryItems.Add(catStringWithCurrent, categoryItem);

                                if (depth == 0)
                                {
                                    characterItem.DropDownItems.Add(categoryItem);
                                }
                                else
                                {
                                    ToolStripMenuItem categoryParent = categoryItems[catString];
                                    categoryParent.DropDownItems.Add(categoryItem);
                                }
                            }

                            MakeCategoriesRecursive(splitCategory, depth + 1);
                        }
                        
                        MakeCategoriesRecursive(character.Category.Split('\\'), 0);
                    }

                    parent = categoryItems[character.Category];
                }

                parent.DropDownItems.Add(characterSelectItem);
            }
        }

        private void SetupAddCharacterItems(ToolStripMenuItem characterItem, ICharacter target)
        {
            IDictionary<string, ToolStripMenuItem> categoryItems = new Dictionary<string, ToolStripMenuItem>();

            IEnumerable<CharacterType> displayedCharacterTypes = GetDisplayedCharacterTypes(characterRegistry.GetAll());
            foreach (CharacterType character in displayedCharacterTypes)
            {
                ToolStripMenuItem characterSelectItem = new ToolStripMenuItem(character.Name, null, (o, e) => MessageBus.Send(new AddCharacterRequestedMessage { Character = character, Target = target}));
                
                ToolStripMenuItem parent;
                if (character.Category == null)
                {
                    parent = characterItem;
                }
                else
                {
                    if (!categoryItems.ContainsKey(character.Category))
                    {
                        void MakeCategoriesRecursive(string[] splitCategory, int depth)
                        {
                            if (depth >= splitCategory.Length) return;
                            
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < depth; i++)
                            {
                                sb.Append(splitCategory[i]);
                                sb.Append("\\");
                            }
                            
                            string catString = depth > 0 ? sb.ToString(0, sb.Length - 1) : "";
                            string catStringWithCurrent = depth > 0 ? $"{catString}\\{splitCategory[depth]}" : splitCategory[depth];

                            if (!categoryItems.ContainsKey(catStringWithCurrent))
                            {
                                ToolStripMenuItem categoryItem = new ToolStripMenuItem(splitCategory[depth]);
                                categoryItems.Add(catStringWithCurrent, categoryItem);

                                if (depth == 0)
                                {
                                    characterItem.DropDownItems.Add(categoryItem);
                                }
                                else
                                {
                                    ToolStripMenuItem categoryParent = categoryItems[catString];
                                    categoryParent.DropDownItems.Add(categoryItem);
                                }
                            }

                            MakeCategoriesRecursive(splitCategory, depth + 1);
                        }
                        
                        MakeCategoriesRecursive(character.Category.Split('\\'), 0);
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
                scaleSelectItem.Checked = (int)target.Scale.X == i;
                scaleItem.DropDownItems.Add(scaleSelectItem);
            }
        }

        private void SetupDistractionScaleOptions(ToolStripMenuItem scaleItem)
        {
            for (int i = 1; i < 9; i++)
            {
                var scaleFactor = i;
                ToolStripMenuItem scaleSelectItem = new ToolStripMenuItem($"{i}x", null, (o, e) => MessageBus.Send(new SetDistractionScaleMessage() { Scale = scaleFactor }));
                scaleSelectItem.Checked = globalSettings.DistractionScale == i;
                scaleItem.DropDownItems.Add(scaleSelectItem);
            }
        }

        private void SetupWindowSelectItems(ToolStripMenuItem parent, ICharacter target)
        {
            ToolStripMenuItem noneItem = new ToolStripMenuItem("None", null, (o, e) =>
            {
                MessageBus.Send(new ChangeContainingWindowMessage() { Window = null, Target = target });
            });
            
            noneItem.Checked = target.TrackedWindow == null;
            parent.DropDownItems.Add(noneItem);

            parent.DropDownOpened += (sender, args) =>
            {
                currentContextMenuStrip.SuspendLayout();
                
                parent.DropDownItems.Clear();
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
                    item.Checked = target.TrackedWindow != null && target.TrackedWindow.Window.hWnd == windowInfo.hWnd;
                    parent.DropDownItems.Add(item);
                }
                
                currentContextMenuStrip.ResumeLayout(true);
            };
        }
        
        private void SetupDistractionWindowSelectItems(ToolStripMenuItem parent)
        {
            ToolStripMenuItem noneItem = new ToolStripMenuItem("None", null, (o, e) =>
            {
                MessageBus.Send(new SetDistractionTrackedWindowMessage() { Window = null });
            });
            
            noneItem.Checked = globalSettings.DistractionWindow == null;
            parent.DropDownItems.Add(noneItem);

            parent.DropDownOpened += (sender, args) =>
            {
                currentContextMenuStrip.SuspendLayout();
             
                parent.DropDownItems.Clear();
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
                        MessageBus.Send(new SetDistractionTrackedWindowMessage() { Window = windowInfo });
                    });
                    item.Checked = globalSettings.DistractionWindow != null && distractionWindowHwnd == windowInfo.hWnd;
                    parent.DropDownItems.Add(item);
                }
                
                currentContextMenuStrip.ResumeLayout(true);
            };
        }

        private void OnClickThroughChangeRequestedMessage(ClickThroughChangeRequestedMessage message)
        {
            if (message.Enabled)
            {
                var result = WindowsUtils.ShowMessageBox("Characters will not react to the mouse in this mode unless CTRL is held down.\n\nDo you want to enable click-through mode?", ProgramInfo.NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    MessageBus.Send(new ClickThroughChangedMessage() { Enabled = false });
                }
                else
                {
                    MessageBus.Send(new ClickThroughChangedMessage() { Enabled = true });
                }
            }
            else
            {
                MessageBus.Send(new ClickThroughChangedMessage() { Enabled = false });
            }
        }

        private void OnSetDistractionTrackedWindowMessage(SetDistractionTrackedWindowMessage message)
        {
            if (message.Window != null) distractionWindowHwnd = message.Window.hWnd;
        }

        private void DuplicateCharacter(ICharacter target, int count)
        {
            for (int i = 0; i < count; i++)
            {
                MessageBus.Send(new AddCharacterRequestedMessage { Character = target.Properties.Type, Target = target });
            }
        }
        
        private void ShowInfoScreen()
        {
            WindowsUtils.ShowMessageBox($"{ProgramInfo.NAME} {ProgramInfo.VERSION}\nCreated by {ProgramInfo.AUTHOR}\n\n{ProgramInfo.CREDITS}\n{ProgramInfo.DISCLAIMER}", "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private IEnumerable<CharacterType> GetDisplayedCharacterTypes(IEnumerable<CharacterType> source)
        {
            return source.Where(t => !t.Hidden).OrderBy(t => t.Order);
        }
    }
}