using System;
using System.Windows.Forms;
using Desktoptale.Messages;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public class ContextMenu : IGameObject
{
    private GameWindow window;
    private InputManager inputManager;
    private GraphicsDevice graphicsDevice;
    
    private int currentScaleFactor;
    private CharacterType currentCharacter;

    public ContextMenu(GameWindow window, InputManager inputManager, GraphicsDevice graphicsDevice)
    {
        this.window = window;
        this.inputManager = inputManager;
        this.graphicsDevice = graphicsDevice;
    }
    
    public void Initialize()
    {
        MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
        MessageBus.Subscribe<CharacterChangeRequestedMessage>(OnCharacterChangeRequestedMessage);
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
        
        ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
        contextMenuStrip.Items.Add(scaleItem);

        for (int i = 1; i < 5; i++)
        {
            var scaleFactor = i;
            ToolStripMenuItem scaleSelectItem = new ToolStripMenuItem($"{i}x", null, (_, _) => MessageBus.Send(new ScaleChangeRequestedMessage { ScaleFactor = scaleFactor }));
            scaleSelectItem.Checked = currentScaleFactor == i;
            scaleItem.DropDownItems.Add(scaleSelectItem);
        }
        
        ToolStripMenuItem characterItem = new ToolStripMenuItem("Character");
        contextMenuStrip.Items.Add(characterItem);
        
        characterItem.DropDownItems.Add(GetCharacterItem(CharacterType.Clover, "Clover"));
        characterItem.DropDownItems.Add(GetCharacterItem(CharacterType.Ceroba, "Ceroba"));
        characterItem.DropDownItems.Add(GetCharacterItem(CharacterType.Martlet, "Martlet"));
        characterItem.DropDownItems.Add(GetCharacterItem(CharacterType.Axis, "Axis"));
        
        ToolStripMenuItem infoItem = new ToolStripMenuItem("About", null, (_, _) => ShowInfoScreen());
        contextMenuStrip.Items.Add(infoItem);
        
        contextMenuStrip.Show(mousePosition.X, mousePosition.Y);
    }

    private ToolStripMenuItem GetCharacterItem(CharacterType characterType, string name)
    {
        ToolStripMenuItem characterSelectItem = new ToolStripMenuItem(name, null, (_, _) => MessageBus.Send(new CharacterChangeRequestedMessage { Character = characterType }));
        characterSelectItem.Checked = currentCharacter == characterType;
        return characterSelectItem;
    }
    
    private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
    {
        currentScaleFactor = (int)message.ScaleFactor;
    }

    private void OnCharacterChangeRequestedMessage(CharacterChangeRequestedMessage message)
    {
        currentCharacter = message.Character;
    }

    private void ShowInfoScreen()
    {
        MessageBox.Show($"{ProgramInfo.NAME} {ProgramInfo.VERSION}\nCreated by {ProgramInfo.AUTHOR}\n\n{ProgramInfo.DESCRIPTION}", "About");
    }

    public void LoadContent(ContentManager contentManager) {}
    public void Draw(GameTime gameTime) {}
    public void Dispose() {}
}