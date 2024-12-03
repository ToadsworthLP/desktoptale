using System;
using System.Collections.Generic;

namespace Desktoptale.Registry
{
    public class Registry<TElement> : IRegistry<TElement, string>
    {
        private readonly IDictionary<string, TElement> elements;
        private readonly IDictionary<TElement, string> reverseMapping;
        
        public Registry()
        {
            elements = new Dictionary<string, TElement>();
            reverseMapping = new Dictionary<TElement, string>();
        }

        public string Add(TElement entry)
        {
            string key = FindKeyForNewElement(entry);
            
            elements.Add(key, entry);
            reverseMapping.Add(entry, key);

            return key;
        }
        
        public string Add(TElement entry, bool overrideExistingEntry)
        {
            string key = overrideExistingEntry ? entry.ToString() : FindKeyForNewElement(entry);
            
            elements[key] = entry;
            reverseMapping[entry] = key;

            return key;
        }

        public bool Contains(string id)
        {
            return elements.ContainsKey(id);
        }

        public TElement Get(string id)
        {
            if (!elements.ContainsKey(id)) throw new IndexOutOfRangeException($"Registry element lookup failed: No element with ID {id} found in registry.");
            
            return elements[id];
        }

        public string GetId(TElement element)
        {
            if (!reverseMapping.ContainsKey(element)) throw new IndexOutOfRangeException($"Registry identifier lookup failed: Element {element} not found in registry.");
            
            return reverseMapping[element];
        }

        public ICollection<TElement> GetAll()
        {
            return elements.Values;
        }

        public ICollection<string> GetAllIds()
        {
            return elements.Keys;
        }

        private string FindKeyForNewElement(TElement entry)
        {
            string entryName = entry.ToString();
            if (!elements.ContainsKey(entryName))
            {
                return entryName;
            }
            
            int i = 0;
            string key;
            do
            {
                key = $"{entryName}{i}";
            } while (elements.ContainsKey(key));

            return key;
        }
    }
}