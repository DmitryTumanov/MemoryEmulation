using System;
using System.Collections.Generic;
using System.Linq;
using MemoryEmulation.DataContext.Enums;
using MemoryEmulation.DataContext.Models;

namespace MemoryEmulation.BusinessLogic.MemoryRepairers.Implementations
{
    public class RandomMemoryRepairer : IMemoryRepairer
    {
        public string Name => "Random repairer #1";

        public string Description => "Fully random!";

        public IEnumerable<int> GetBrokenBitsIndexes(IEnumerable<Bit> memory, int neededBitsCount)
        {
            var memoryList = memory.ToList();
            var randomMemory = memoryList.AsEnumerable().OrderBy(n => Guid.NewGuid()).Take(neededBitsCount);

            foreach (var bit in randomMemory)
            {
                if (bit.State != BitStates.Broken)
                {
                    continue;
                }

                yield return memoryList.IndexOf(bit);
            }
        }

        public void Refresh()
        {
        }
    }
}
