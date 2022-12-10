using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife
{
    internal class GameState
    {
        public bool[,] GameStates = new bool[ulong.MaxValue, ulong.MaxValue];


        public bool GetState(long x, long y, bool[,] state)
        {
            return state[x.Map(), y.Map()];
        }

        /// <summary>
        /// If an "alive" cell had less than 2 or more than 3 alive neighbors (in any of the 8 surrounding cells), it becomes dead.
        /// If a "dead" cell had *exactly* 3 alive neighbors, it becomes alive.
        /// </summary>
        internal bool CalculateNextState(ulong x, ulong y, bool[,] state)
        {
            // TODO: bounds checks
            
            // (x + 1, y + 1)
            // (x + 1, y)
            // (x + 1, y - 1)
            // (x, y + 1)

            // (x, y - 1)
            // (x - 1, y + 1)
            // (x - 1, y)
            // (x - 1, y - 1)

            for (ulong i = x - 1; i >= x + 1; i++)
            {
                for (ulong j = y -1; j >= y + 1; j++)
                {
                    Console.WriteLine($"{i}, {j}");
                }
            }

            return true;
        }
    }


}
