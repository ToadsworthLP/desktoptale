using System.Collections.Generic;

namespace Desktoptale.Registry
{
    public interface IRegistry<TElement, TIdentifier>
    {
        TIdentifier Add(TElement entry);
        TIdentifier Add(TElement entry, bool overrideExistingEntry);
        TElement Get(TIdentifier id);
        TIdentifier GetId(TElement element);
        ICollection<TElement> GetAll();
        ICollection<TIdentifier> GetAllIds();
    }
}