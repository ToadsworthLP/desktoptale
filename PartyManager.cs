using System.Collections.Generic;

namespace Desktoptale
{
    public class PartyManager
    {
        private IDictionary<string, Party> parties;

        public PartyManager()
        {
            this.parties = new Dictionary<string, Party>();
        }

        public bool IsNewPartyNameValid(string name)
        {
            bool valid = !string.IsNullOrWhiteSpace(name) && name.Length <= 50;

            if (valid)
            {
                return !parties.ContainsKey(name);
            }

            return false;
        }
        
        public Party GetOrCreateParty(string name)
        {
            if (parties.ContainsKey(name))
            {
                return parties[name];
            }

            Party party = new Party(this, name);
            party.PartyDissolved += () => { parties.Remove(name); };
            parties.Add(name, party);

            return party;
        }

        public IEnumerable<Party> GetAllParties()
        {
            return parties.Values;
        }
    }
}