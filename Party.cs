using System.Collections.Generic;
using System.Linq;

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

        public void Remove(ICharacter character, bool skipPartyCleanup = false)
        {
            int removedIndex = members[character];
            members.Remove(character);

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
            }

            nextIndex--;

            reverseMapping = members.ToDictionary(e => e.Value, e => e.Key);

            if (!skipPartyCleanup)
            {
                if (members.Count == 0 && reverseMapping.Count == 0)
                {
                    PartyDissolved?.Invoke();
                }
            }
        }

        public void RemoveWithFreeSpace(ICharacter character)
        {
            int removedIndex = members[character];
            members.Remove(character);
            reverseMapping[removedIndex] = null;
        }
        
        public void InsertIntoFreeSpace(ICharacter character)
        {
            int freeSpaceIndex = -1;
            foreach (KeyValuePair<int, ICharacter> entry in reverseMapping)
            {
                if (entry.Value == null) freeSpaceIndex = entry.Key;
            }
            
            members.Add(character, freeSpaceIndex);
            reverseMapping[freeSpaceIndex] = character;
        }

        public void MakeLeader(ICharacter character)
        {
            Remove(character, true);
            
            IList<ICharacter> affectedMembers = new List<ICharacter>(members.Keys);
            foreach (ICharacter member in affectedMembers)
            {
                int updatedIndex = members[member] + 1;
                members[member] = updatedIndex;
            }
            
            members.Add(character, 0);
            reverseMapping = members.ToDictionary(e => e.Value, e => e.Key);

            nextIndex++;
        }

        public ICharacter GetCharacterInFront(ICharacter character)
        {
            int charIndex = members[character];
            if (reverseMapping.TryGetValue(charIndex - 1, out var inFront))
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