using System;
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
    public class ContextMenu : IGameObject
    {
        private GameWindow window;
        private InputManager inputManager;
        private GraphicsDevice graphicsDevice;
        private IRegistry<CharacterType, int> characterRegistry;
        
        private int currentScaleFactor;
        private CharacterType currentCharacter;
        private bool idleMovementEnabled = true;
        private bool unfocusedMovementEnabled = false;

        public ContextMenu(GameWindow window, InputManager inputManager, GraphicsDevice graphicsDevice, IRegistry<CharacterType, int> characterRegistry)
        {
            this.window = window;
            this.inputManager = inputManager;
            this.graphicsDevice = graphicsDevice;
            this.characterRegistry = characterRegistry;
        }
        
        public void Initialize()
        {
            MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
            MessageBus.Subscribe<CharacterChangeSuccessMessage>(OnCharacterChangeSuccessMessage);
            MessageBus.Subscribe<IdleMovementChangeRequestedMessage>(OnIdleMovementChangeRequestedMessage);
            MessageBus.Subscribe<UnfocusedMovementChangeRequestedMessage>(OnUnfocusedMovementChangeRequestedMessage);
        }
        
        public void Update(GameTime gameTime)
        {
            if (inputManager.RightClickJustPressed &&
                graphicsDevice.Viewport.Bounds.Contains(inputManager.PointerPosition))
            {
                OpenContextMenu(window.Position + inputManager.PointerPosition);
            }
        }

        private void OpenContextMenu(Point mousePosition)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            
            ToolStripMenuItem characterItem = new ToolStripMenuItem("Character");
            contextMenuStrip.Items.Add(characterItem);

            IDictionary<string, ToolStripMenuItem> categoryItems = new Dictionary<string, ToolStripMenuItem>();

            foreach (CharacterType character in characterRegistry.GetAll())
            {
                ToolStripMenuItem characterSelectItem = new ToolStripMenuItem(character.Name, null, (o, e) => MessageBus.Send(new CharacterChangeRequestedMessage { Character = character }));
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
            
            ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
            contextMenuStrip.Items.Add(scaleItem);

            for (int i = 1; i < 5; i++)
            {
                var scaleFactor = i;
                ToolStripMenuItem scaleSelectItem = new ToolStripMenuItem($"{i}x", null, (o, e) => MessageBus.Send(new ScaleChangeRequestedMessage { ScaleFactor = scaleFactor }));
                scaleSelectItem.Checked = currentScaleFactor == i;
                scaleItem.DropDownItems.Add(scaleSelectItem);
            }
            
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            contextMenuStrip.Items.Add(settingsItem);
            
            ToolStripMenuItem idleMovementItem = new ToolStripMenuItem($"Idle Roaming", null, (o, e) => MessageBus.Send(new IdleMovementChangeRequestedMessage { Enabled = !idleMovementEnabled}));
            idleMovementItem.Checked = idleMovementEnabled;
            settingsItem.DropDownItems.Add(idleMovementItem);
            
            ToolStripMenuItem unfocusedMovementItem = new ToolStripMenuItem($"Unfocused Movement", null, (o, e) => MessageBus.Send(new UnfocusedMovementChangeRequestedMessage { Enabled = !unfocusedMovementEnabled}));
            unfocusedMovementItem.Checked = unfocusedMovementEnabled;
            settingsItem.DropDownItems.Add(unfocusedMovementItem);
            
            ToolStripMenuItem infoItem = new ToolStripMenuItem("About", null, (o, e) => ShowInfoScreen());
            contextMenuStrip.Items.Add(infoItem);
            
            contextMenuStrip.Show(mousePosition.X, mousePosition.Y);
        }
        
        private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
        {
            currentScaleFactor = (int)message.ScaleFactor;
        }

        private void OnCharacterChangeSuccessMessage(CharacterChangeSuccessMessage message)
        {
            currentCharacter = message.Character;
        }
        
        private void OnIdleMovementChangeRequestedMessage(IdleMovementChangeRequestedMessage message)
        {
            idleMovementEnabled = message.Enabled;
        }
        
        private void OnUnfocusedMovementChangeRequestedMessage(UnfocusedMovementChangeRequestedMessage message)
        {
            unfocusedMovementEnabled = message.Enabled;
        }

        private void ShowInfoScreen()
        {
            MessageBox.Show($"{ProgramInfo.NAME} {ProgramInfo.VERSION}\nCreated by {ProgramInfo.AUTHOR}\n\n{ProgramInfo.CREDITS}\n\n{ProgramInfo.DISCLAIMER}", "About");
        }

        public void LoadContent(ContentManager contentManager) {}
        public void Draw(GameTime gameTime) {}
        public void Dispose() {}
    }
}