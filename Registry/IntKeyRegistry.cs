using System;
using System.Collections.Generic;

namespace Desktoptale.Registry
{
    public class IntKeyRegistry<TElement> : IRegistry<TElement, int>
    {
        private readonly IDictionary<int, TElement> elements;
        private readonly IDictionary<TElement, int> reverseMapping;

        private int nextKey;
        
        public IntKeyRegistry()
        {
            elements = new Dictionary<int, TElement>();
            reverseMapping = new Dictionary<TElement, int>();

            nextKey = 0;
        }

        public int Add(TElement entry)
        {
            elements.Add(nextKey, entry);
            reverseMapping.Add(entry, nextKey);

            return nextKey++;
        }

        public TElement Get(int id)
        {
            if (!elements.ContainsKey(id)) throw new IndexOutOfRangeException($"Registry element lookup failed: No element with ID {id} found in registry.");
            
            return elements[id];
        }

        public int GetId(TElement element)
        {
            if (!reverseMapping.ContainsKey(element)) throw new IndexOutOfRangeException($"Registry identifier lookup failed: Element {element} not found in registry.");
            
            return reverseMapping[element];
        }

        public ICollection<TElement> GetAll()
        {
            return elements.Values;
        }
    }
}