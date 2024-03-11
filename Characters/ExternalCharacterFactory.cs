using System;
using System.IO;
using System.Windows.Forms;
using Desktoptale.Registry;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Desktoptale.Characters
{
    public class ExternalCharacterFactory
    {
        private string rootPath;
        private SpriteCache spriteCache;
        private GraphicsDevice graphicsDevice;

        public ExternalCharacterFactory(string rootPath, GraphicsDevice graphicsDevice)
        {
            this.rootPath = rootPath;
            this.graphicsDevice = graphicsDevice;
            spriteCache = new SpriteCache(this.graphicsDevice);
        }

        public void AddAllToRegistry(IRegistry<CharacterType, int> registry)
        {
            if (!Directory.Exists(rootPath))
            {
                return;
            }
            
            foreach (string jsonPath in Directory.EnumerateFiles(rootPath, "*.json", SearchOption.AllDirectories))
            {
                string json;
                try
                {
                    json = File.ReadAllText(jsonPath);
                }
                catch (Exception e)
                {
                    ShowJsonReadError(jsonPath);
                    continue;
                }
                
                AddToRegistry(registry, jsonPath, json);
            }
        }

        private void AddToRegistry(IRegistry<CharacterType, int> registry, string path, string json)
        {
            ExternalCharacterDefinition externalCharacterDefinition;
            try
            {
                externalCharacterDefinition = JsonConvert.DeserializeObject<ExternalCharacterDefinition>(json);
            }
            catch (Exception e)
            {
                ShowJsonParseError(path, e.Message);
                return;
            }

            if (!ValidateCharacterDefinition(externalCharacterDefinition))
                return;

            string basePath = new FileInfo(path).Directory.FullName;
            CharacterType characterType = new CharacterType(externalCharacterDefinition.Name, externalCharacterDefinition.Category, context =>
            {
                ExternalCharacter externalCharacter = new ExternalCharacter(context);
                SetupState((s) => externalCharacter.IdleSprite = s, externalCharacterDefinition.Idle, basePath);
                SetupState((s) => externalCharacter.WalkSprite = s, externalCharacterDefinition.Walk, basePath);

                if (externalCharacterDefinition.Run == null)
                {
                    SetupState((s) => externalCharacter.RunSprite = s, externalCharacterDefinition.Walk, basePath);
                    externalCharacter.RunSprite.Framerate = externalCharacter.WalkSprite.Framerate * 2;
                }
                else
                {
                    SetupState((s) => externalCharacter.RunSprite = s, externalCharacterDefinition.Run, basePath);
                }
                
                return externalCharacter;
            });

            registry.Add(characterType);
        }

        private void SetupState(Action<IAnimatedSprite> setter, ExternalCharacterDefinition.ExternalCharacterStateDefinition state, string basePath)
        {
            OrientedAnimatedSprite sprite;
            if (state.Right == null)
            {
                sprite = new OrientedAnimatedSprite(
                    GetSpriteForOrientation(state.Up, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Left, basePath)
                );
            }
            else
            {
                sprite = new OrientedAnimatedSprite(
                    GetSpriteForOrientation(state.Up, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Left, basePath),
                    GetSpriteForOrientation(state.Right, basePath)
                );
            }
            
            setter.Invoke(sprite);
        }

        private AnimatedSprite GetSpriteForOrientation(ExternalCharacterDefinition.ExternalCharacterSpriteDefinition spriteDefinition, string basePath)
        {
            string absolutePath = Path.Combine(basePath, spriteDefinition.Sprite);

            Texture2D texture;
            try
            {
                texture = spriteCache.Get(absolutePath);
            }
            catch (Exception e)
            {
                ShowSpriteLoadError(absolutePath);
                throw e;
            }
            
            AnimatedSprite sprite = new AnimatedSprite(texture, spriteDefinition.FrameCount);
            sprite.Framerate = spriteDefinition.FrameRate;
            sprite.Loop = spriteDefinition.Loop;
            sprite.StartFrame = spriteDefinition.StartFrame;

            return sprite;
        }

        private bool ValidateCharacterDefinition(ExternalCharacterDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.Name))
            {
                ShowDefinitionError("Unknown", "No or invalid character name set. Character names cannot be empty or contain only whitespace.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(definition.Category))
            {
                definition.Category = null;
            }

            if (definition.Idle == null)
            {
                ShowDefinitionError(definition.Name, "No Idle state set.");
                return false;
            }

            if (definition.Walk == null)
            {
                definition.Walk = definition.Idle;
            }

            if (definition.Run == null)
            {
                return ValidateCharacterStateDefinition(definition.Idle, definition.Name, "Idle") &&
                       ValidateCharacterStateDefinition(definition.Walk, definition.Name, "Walk");
            }

            return ValidateCharacterStateDefinition(definition.Idle, definition.Name, "Idle") &&
                   ValidateCharacterStateDefinition(definition.Walk, definition.Name, "Walk") &&
                   ValidateCharacterStateDefinition(definition.Run, definition.Name, "Run");
        }

        private bool ValidateCharacterStateDefinition(ExternalCharacterDefinition.ExternalCharacterStateDefinition state, string characterName, string stateName)
        {
            if (state.Down == null)
            {
                ShowStateDefinitionError(characterName, stateName, "Orientation Down undefined.");
                return false;
            }

            if (state.Up == null)
            {
                ShowStateDefinitionError(characterName, stateName, "Orientation Up undefined.");
                return false;
            }

            if (state.Left == null)
            {
                ShowStateDefinitionError(characterName, stateName, "Orientation Left undefined.");
                return false;
            }

            if (state.Right == null)
            {
                return ValidateCharacterSpriteDefinition(state.Up, characterName, stateName, "Up") &&
                       ValidateCharacterSpriteDefinition(state.Down, characterName, stateName, "Down") &&
                       ValidateCharacterSpriteDefinition(state.Left, characterName, stateName, "Left");
            }
            else
            {
                return ValidateCharacterSpriteDefinition(state.Up, characterName, stateName, "Up") &&
                       ValidateCharacterSpriteDefinition(state.Down, characterName, stateName, "Down") &&
                       ValidateCharacterSpriteDefinition(state.Left, characterName, stateName, "Left") &&
                       ValidateCharacterSpriteDefinition(state.Right, characterName, stateName, "Right");
            }
        }

        private bool ValidateCharacterSpriteDefinition(ExternalCharacterDefinition.ExternalCharacterSpriteDefinition sprite, string characterName, string stateName, string orientation)
        {
            if (sprite.Sprite == null)
            {
                ShowSpriteDefinitionError(characterName, stateName, orientation, "No sprite path set.");
                return false;
            }
            
            return true;
        }

        private void ShowSpriteLoadError(string path)
        {
            MessageBox.Show($"Failed to load sprite at {path}.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowJsonReadError(string path)
        {
            MessageBox.Show($"Failed to read JSON file {path}.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowJsonParseError(string path, string error)
        {
            MessageBox.Show($"Failed to parse JSON file {path}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowDefinitionError(string name, string error)
        {
            MessageBox.Show($"Failed to load character {name}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        private void ShowStateDefinitionError(string characterName, string stateName, string error)
        {
            MessageBox.Show($"Failed to load state {stateName} of character {characterName}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        private void ShowSpriteDefinitionError(string characterName, string stateName, string orientation, string error)
        {
            MessageBox.Show($"Failed to load sprite {orientation} of state {stateName} of character {characterName}:\n{error}","Custom Character Loader" , MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}