using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife
{
    internal static class MappingExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Map(this long value)
        {
            return unchecked((ulong)(value - long.MinValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Map(this ulong value)
        {
            return unchecked((long)value + long.MinValue);
        }
    }
}
