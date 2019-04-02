using System.Collections.Generic;
using MemoryEmulation.DataContext.Models;

namespace MemoryEmulation.BusinessLogic.MemoryRepairers
{
    public interface IMemoryRepairer
    {
        string Name { get; }

        string Description { get; }

        IEnumerable<int> GetBrokenBitsIndexes(IEnumerable<Bit> memory, int neededBitsCount);

        void Refresh();
    }
}
