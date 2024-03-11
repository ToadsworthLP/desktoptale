using System;
using System.IO;
using System.Windows.Forms;
using Desktoptale.Registry;
using Newtonsoft.Json;

namespace Desktoptale.Characters
{
    public class ExternalCharacterFactory
    {
        private string rootPath;

        public ExternalCharacterFactory(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public void AddAllToRegistry(IRegistry<CharacterType, int> registry)
        {
            if (!Directory.Exists(rootPath))
            {
                try
                {
                    Directory.CreateDirectory(rootPath);
                }
                catch (Exception e)
                {
                    ShowDirectoryError();
                }
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

            CharacterType characterType = new CharacterType(externalCharacterDefinition.Name, externalCharacterDefinition.Category, context =>
            {
                ExternalCharacter externalCharacter = new ExternalCharacter(context);
                
                // TODO set up sprites and cache them so avoid loading the same one multiple times
                
                return externalCharacter;
            });

            registry.Add(characterType);
        }

        private bool ValidateCharacterDefinition(ExternalCharacterDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.Name))
            {
                ShowDefinitionError("Unknown", "No or invalid character name set. Character names cannot be empty or contain only whitespace.");
                return false;
            }

            if (definition.Category == null)
            {
                definition.Category = "";
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
                definition.Run = definition.Walk;
            }

            return ValidateCharacterStateDefinition(definition.Idle, definition.Name, "Idle") &&
                   ValidateCharacterStateDefinition(definition.Walk, definition.Name, "Walk") &&
                   ValidateCharacterStateDefinition(definition.Run, definition.Name, "Run");
        }

        private bool ValidateCharacterStateDefinition(ExternalCharacterDefinition.ExternalCharacterStateDefinition state, string characterName, string stateName)
        {
            if (state.Down == null)
            {
                ShowStateDefinitionError(characterName, stateName, "Up orientation undefined.");
                return false;
            }

            if (state.Up == null)
            {
                state.Up = state.Down;
            }

            if (state.Left == null)
            {
                state.Left = state.Down;
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

        private void ShowDirectoryError()
        {
            MessageBox.Show("Failed to create custom character directory.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowJsonReadError(string path)
        {
            MessageBox.Show($"Failed to read JSON file {path}.", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowJsonParseError(string path, string error)
        {
            MessageBox.Show($"Failed to parse JSON file {path}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowDefinitionError(string name, string error)
        {
            MessageBox.Show($"Failed to load character {name}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowStateDefinitionError(string characterName, string stateName, string error)
        {
            MessageBox.Show($"Failed to load state {stateName} of character {characterName}:\n{error}", "Custom Character Loader", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowSpriteDefinitionError(string characterName, string stateName, string orientation, string error)
        {
            MessageBox.Show($"Failed to load sprite {orientation} of state {stateName} of character {characterName}:\n{error}","Custom Character Loader" , MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}