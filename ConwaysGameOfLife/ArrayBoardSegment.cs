using System;
using System.Collections.Generic;

namespace ConwaysGameOfLife
{
    internal class ArrayBoardSegment
    {
        private readonly long xPosition;
        private readonly long yPosition;
        private readonly int size;

        private bool[,]? state;
        private List<(int, int)> UpdatesToApply = new();

        public ArrayBoardSegment(long xPosition, long yPosition, int size)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.size = size;
        }

        public void Initialize(IReadOnlyList<(int, int)> updatesToApply)
        {
            foreach (var update in updatesToApply)
            {
                if (this.state is null)
                {
                    this.state = new bool[size, size];
                }

                if (update.Item1 > size || update.Item2 > size)
                {
                    throw new ArgumentOutOfRangeException(nameof(update));
                }

                this.state[update.Item1, update.Item2] = true;
            }
        }

        internal void CalculateNextState()
        {
            this.UpdatesToApply = new List<(int, int)> ();
            for(int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (this.ShouldNodeUpdate(x, y))
                    {
                        this.UpdatesToApply.Add((x, y));
                    }
                }
            }
        }

        internal void ApplyUpdates()
        {
            foreach (var update in this.UpdatesToApply)
            {
                if (this.state is null)
                {
                    this.state = new bool[size, size];
                }

                this.state[update.Item1, update.Item2] = !this.state[update.Item1, update.Item2];
            }
        }

        /// <summary>
        /// If an "alive" cell had less than 2 or more than 3 alive neighbors (in any of the 8 surrounding cells), it becomes dead.
        /// If a "dead" cell had *exactly* 3 alive neighbors, it becomes alive.
        /// </summary>
        internal bool ShouldNodeUpdate(int x, int y)
        {
            var currentlyAlive = this.IsNodeAlive(x, y);
            var aliveCount = 0;

            // Iterate through the 8 surrounding nodes and find which ones are alive.
            for (var xCoordinate = x - 1; xCoordinate <= x + 1; xCoordinate++)
            {
                for (var yCoordinate = y - 1; yCoordinate <= y + 1; yCoordinate++)
                {
                    // Don't check self
                    if (xCoordinate == x && yCoordinate == y)
                    {
                        continue;
                    }

                    // Check if the neighbor node is alive
                    if (this.IsNodeAlive(xCoordinate, yCoordinate))
                    {
                        aliveCount++;
                    }

                    // Short circuit if we're currently alive and more than 3 neighbor nodes are alive
                    if (currentlyAlive && aliveCount > 3)
                    {
                        return true;
                    }
                }
            }

            // Check if we're currently a dead cell and have exactly 3 alive neighbors
            // or if we're alive and have less than 2 alive neighbors.
            // We don't need to check for currently alive and more than 3 as we would have already returned.
            return (!currentlyAlive && aliveCount == 3) || (currentlyAlive && aliveCount < 2);
        }

        internal bool GetState(long x, long y)
        {
            if (x < xPosition || y < yPosition)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (x > xPosition + size || y > yPosition + size)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.state is not null && state[x - xPosition, y - yPosition];
        }

        private bool IsNodeAlive(int x, int y)
        {
            if (x < 0)
            {
                if (y < 0)
                {
                    // Check corner segment
                    return false;
                }

                if (y >= this.size)
                {
                    // Check corner segment
                    return false;
                }

                // Check other segment
                return false;
            }

            if (x >= this.size)
            {
                if (y < 0)
                {
                    // Check corner segment
                    return false;
                }

                if (y >= this.size)
                {
                    // Check corner segment
                    return false;
                }

                // Check other segment
                return false;
            }

            if (y < 0)
            {
                // Check other segment
                return false;
            }

            if (y >= this.size)
            {
                // Check other segment
                return false;
            }

            try
            {
                return this.state is not null && this.state[x, y];
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"{x}, {y}: {ex}");
                return false;
            }
        }
    }
}
