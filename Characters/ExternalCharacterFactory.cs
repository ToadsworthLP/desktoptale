using System;
using System.IO;
using System.Windows.Forms;
using Desktoptale.Registry;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Desktoptale.Characters
{
    public class ExternalCharacterFactory
    {
        private string rootPath;
        private SpriteCache spriteCache;
        private GraphicsDevice graphicsDevice;
        private IDeserializer deserializer;

        public ExternalCharacterFactory(string rootPath, GraphicsDevice graphicsDevice)
        {
            this.rootPath = Path.GetFullPath(rootPath);
            this.graphicsDevice = graphicsDevice;
            spriteCache = new SpriteCache(this.graphicsDevice);

            deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }

        public void AddAllToRegistry(IRegistry<CharacterType, string> registry)
        {
            if (!Directory.Exists(rootPath))
            {
                return;
            }
            
            foreach (string definitionPath in FileEnumerator.EnumerateFilesRecursive(rootPath, "*.yaml"))
            {
                string definitionString;
                try
                {
                    definitionString = File.ReadAllText(definitionPath);
                }
                catch (Exception e)
                {
                    ShowDefinitionReadError(definitionPath);
                    continue;
                }
                
                AddToRegistry(registry, definitionPath, definitionString);
            }
        }

        private void AddToRegistry(IRegistry<CharacterType, string> registry, string path, string definitionString)
        {
            ExternalCharacterDefinition externalCharacterDefinition;
            try
            {
                externalCharacterDefinition = deserializer.Deserialize<ExternalCharacterDefinition>(definitionString);
            }
            catch (Exception e)
            {
                ShowDefinitionParseError(path, e.Message);
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

                if (externalCharacterDefinition.Drag != null)
                {
                    SetupState((s) => externalCharacter.DragSprite = s, externalCharacterDefinition.Drag, basePath);
                }
                
                if (externalCharacterDefinition.Action != null)
                {
                    SetupState((s) => externalCharacter.ActionSprite = s, externalCharacterDefinition.Action, basePath);
                }
                
                if (externalCharacterDefinition.Disappear != null)
                {
                    SetupState((s) => externalCharacter.DisappearSprite = s, externalCharacterDefinition.Disappear, basePath);
                }
                
                if (externalCharacterDefinition.Appear != null)
                {
                    SetupState((s) => externalCharacter.AppearSprite = s, externalCharacterDefinition.Appear, basePath);
                }
                
                if (externalCharacterDefinition.Spawn != null)
                {
                    SetupState((s) => externalCharacter.SpawnSprite = s, externalCharacterDefinition.Spawn, basePath);
                }
                
                return externalCharacter;
            });
            
            if (externalCharacterDefinition.WalkSpeed.HasValue)
            {
                characterType.WalkSpeed = externalCharacterDefinition.WalkSpeed.Value;
                characterType.RunSpeed = externalCharacterDefinition.WalkSpeed.Value * 2f;
            }
            
            if (externalCharacterDefinition.RunSpeed.HasValue)
            {
                characterType.RunSpeed = externalCharacterDefinition.RunSpeed.Value;
            }
            
            if (externalCharacterDefinition.Hidden.HasValue)
            {
                characterType.Hidden = externalCharacterDefinition.Hidden.Value;
            }
            
            if (externalCharacterDefinition.Teleport.HasValue)
            {
                characterType.Teleport = externalCharacterDefinition.Teleport.Value;
            }
            
            if (externalCharacterDefinition.Override == true && registry.Contains(characterType.ToString()) && registry.Get(characterType.ToString()).BuiltIn)
            {
                characterType.BuiltIn = true;
            }
            else
            {
                characterType.BuiltIn = false;
            }
            
            registry.Add(characterType, externalCharacterDefinition.Override.GetValueOrDefault(false));
        }

        private void SetupState(Action<IAnimatedSprite> setter, ExternalCharacterDefinition.ExternalCharacterStateDefinition state, string basePath)
        {
            OrientedAnimatedSprite sprite;
            if (state.Left == null && state.Right == null && state.Up == null && state.Down != null)
            {
                sprite = new OrientedAnimatedSprite(
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    true
                );
            } 
            else if (state.Right == null)
            {
                sprite = new OrientedAnimatedSprite(
                    GetSpriteForOrientation(state.Up, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Left, basePath),
                    GetSpriteForOrientation(state.Left, basePath),
                    true
                );
            }
            else
            {
                sprite = new OrientedAnimatedSprite(
                    GetSpriteForOrientation(state.Up, basePath),
                    GetSpriteForOrientation(state.Down, basePath),
                    GetSpriteForOrientation(state.Left, basePath),
                    GetSpriteForOrientation(state.Right, basePath),
                    false
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
            sprite.LoopPoint = spriteDefinition.LoopPoint;

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
            if (state.Left == null && state.Right == null && state.Up == null && state.Down != null)
            {
                return ValidateCharacterSpriteDefinition(state.Down, characterName, stateName, "Down");
            }
            
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
            WindowsUtils.ShowMessageBox($"Failed to load sprite at {path}.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowDefinitionReadError(string path)
        {
            WindowsUtils.ShowMessageBox($"Failed to read definition file {path}.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowDefinitionParseError(string path, string error)
        {
            WindowsUtils.ShowMessageBox($"Failed to parse definition file {path}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowDefinitionError(string name, string error)
        {
            WindowsUtils.ShowMessageBox($"Failed to load character {name}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        private void ShowStateDefinitionError(string characterName, string stateName, string error)
        {
            WindowsUtils.ShowMessageBox($"Failed to load state {stateName} of character {characterName}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        private void ShowSpriteDefinitionError(string characterName, string stateName, string orientation, string error)
        {
            WindowsUtils.ShowMessageBox($"Failed to load sprite {orientation} of state {stateName} of character {characterName}:\n{error}","Custom Character Loader" , MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}