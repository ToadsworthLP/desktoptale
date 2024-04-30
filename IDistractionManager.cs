using Desktoptale.Distractions;

namespace Desktoptale
{
    public interface IDistractionManager
    {
        void AddDistraction(IDistraction distraction);
        void RemoveDistraction(IDistraction distraction);
    }
}