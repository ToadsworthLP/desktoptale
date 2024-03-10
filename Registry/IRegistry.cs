using System.Collections.Generic;

namespace Desktoptale.Registry
{
    public interface IRegistry<TElement, TIdentifier>
    {
        TIdentifier Add(TElement entry);
        TElement Get(TIdentifier id);
        TIdentifier GetId(TElement element);
        ICollection<TElement> GetAll();
    }
}