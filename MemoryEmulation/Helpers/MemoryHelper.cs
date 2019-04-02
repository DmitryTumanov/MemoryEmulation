using System;
using System.Collections.Generic;
using System.Linq;
using MemoryEmulation.ViewModels;

namespace MemoryEmulation.Helpers
{
    public class MemoryHelper
    {
        public static IEnumerable<BitViewModel> GetRandomBits(IEnumerable<BitViewModel> memory, int bitsCount)
        {
            return memory.AsEnumerable().OrderBy(n => Guid.NewGuid()).Take((int)bitsCount);
        }
    }
}
