using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale;

public class CharacterFactory
{
    public Character Create(CharacterType type, GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager)
    {
        Character character;

        switch (type)
        {
            case CharacterType.Clover:
                character = new Clover(graphics, window, spriteBatch, inputManager);
                break;
            case CharacterType.Ceroba:
                character = new Ceroba(graphics, window, spriteBatch, inputManager);
                break;
            case CharacterType.Martlet:
                character = new Martlet(graphics, window, spriteBatch, inputManager);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        return character;
    }
}

public enum CharacterType { Clover, Ceroba, Martlet }