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
            reverseMapping = new Dictionary<int, ICharacter>();
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
            reverseMapping.Remove(removedIndex);

            ICollection<ICharacter> allMembers = members.Keys;
            IList<ICharacter> affectedMembers = new List<ICharacter>();
            
            foreach (ICharacter member in allMembers)
            {
                int memberIndex = members[member];
                if (memberIndex > removedIndex) affectedMembers.Add(member);
            }

            foreach (ICharacter affectedMember in affectedMembers)
            {
                int updatedIndex = members[affectedMember] - 1;
                members[affectedMember] = updatedIndex;
                reverseMapping.Remove(updatedIndex + 1);
                reverseMapping[updatedIndex] = affectedMember;
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