using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MemoryEmulation.DataContext.Enums;
using MemoryEmulation.DataContext.Models;

namespace MemoryEmulation.BusinessLogic.Generators
{
    public static class MemoryGenerator
    {
        public static IEnumerable<Bit> CreateGenerator(long bitsCount)
        {
            for (var _ = 0; _ < bitsCount; _++)
            {
                yield return new Bit
                {
                    State = BitStates.Zero
                };
            }
        }

        public static IEnumerable<Bit> GetBiteArrayFromStream(Stream stream)
        {
            var biteArray = new BitArray(GetByteArrayFromStream(stream));

            return (from bool b in biteArray
                select new Bit
                {
                    State = b ? BitStates.One : BitStates.Zero
                }).ToList();
        }

        private static byte[] GetByteArrayFromStream(Stream stream)
        {
            var result = new List<byte>();
            var fitstByte = stream.ReadByte();
            while (fitstByte != -1)
            {
                result.Add((byte)fitstByte);
                fitstByte = stream.ReadByte();
            }

            return result.ToArray();
        }
    }
}
