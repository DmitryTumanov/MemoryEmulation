using System;
using System.Collections.Generic;
using System.Linq;
using MemoryEmulation.DataContext.Enums;
using MemoryEmulation.DataContext.Models;

namespace MemoryEmulation.BusinessLogic.MemoryRepairers.Implementations
{
    public class RandomWithPositionSavingMemoryRepairer:IMemoryRepairer
    {
        private readonly List<int> _brokenBitsIndexes = new List<int>();

        public string Name => "Random repairer #2";

        public string Description => "Random, but it's remember positions of checked bits";

        public IEnumerable<int> GetBrokenBitsIndexes(IEnumerable<Bit> memory, int neededBitsCount)
        {
            var memoryList = memory.ToList();
            var alreadyCheckedBits = _brokenBitsIndexes.Select(x => memoryList.ElementAt(x));
            var randomMemory = memoryList.Except(alreadyCheckedBits).AsEnumerable().OrderBy(n => Guid.NewGuid())
                .Take(neededBitsCount);

            foreach (var bit in randomMemory)
            {
                var indexOfBit = memoryList.IndexOf(bit);
                _brokenBitsIndexes.Add(indexOfBit);
                if (bit.State != BitStates.Broken)
                {
                    continue;
                }

                yield return indexOfBit;
            }
        }

        public void Refresh()
        {
            _brokenBitsIndexes.Clear();
        }
    }
}
