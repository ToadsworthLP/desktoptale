using System.Collections.Generic;

namespace Desktoptale
{
    public class Party
    {
        public delegate void PartyDissolvedEventHandler();
        public event PartyDissolvedEventHandler PartyDissolved;
        
        public string Name { get; private set; }
        
        private IDictionary<ICharacter, int> members;
        private IDictionary<int, ICharacter> reverseMapping;

        private int nextIndex = 0;

        public Party(PartyManager partyManager, string name)
        {
            this.Name = name;

            members = new Dictionary<ICharacter, int>();
        }

        public void Add(ICharacter character)
        {
            members.Add(character, nextIndex);
            reverseMapping.Add(nextIndex, character);
            nextIndex++;
        }

        public void Remove(ICharacter character)
        {
            int removedIndex = members[character];
            members.Remove(character);

            IEnumerable<ICharacter> allMembers = members.Keys;
            foreach (ICharacter member in allMembers)
            {
                int memberIndex = members[member];
                int updatedIndex = memberIndex;
                if (memberIndex > removedIndex)
                {
                    updatedIndex = memberIndex--;
                    members[member] = updatedIndex;
                }

                reverseMapping.Remove(memberIndex);
                reverseMapping.Add(updatedIndex, character);
            }

            nextIndex--;

            if (members.Count == 0)
            {
                PartyDissolved?.Invoke();
            }
        }

        public void MakeLeader(ICharacter character)
        {
            // TODO implement this
        }

        public ICharacter GetCharacterInFront(ICharacter character)
        {
            int charIndex = members[character];
            if (reverseMapping.TryGetValue(charIndex + 1, out var inFront))
            {
                return inFront;
            }
            else
            {
                return null;
            }
        }

        public ICharacter GetLeader()
        {
            return reverseMapping[0];
        }
    }
}