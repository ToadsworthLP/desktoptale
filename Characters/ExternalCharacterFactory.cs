using Desktoptale.Registry;

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
            // TODO Read character definitions recursively from rootPath and add them to the registry, then figure out a way to cache sprites and only load them on demand
        }
    }
}