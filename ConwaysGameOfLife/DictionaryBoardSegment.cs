using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ConwaysGameOfLife
{
    internal record UpdatedCell(long xPosition, long yPosition, bool isAlive);

    internal class DictionaryBoardSegment
    {
        private readonly long xMin = long.MinValue;
        private readonly long xMax = long.MaxValue;

        private readonly long yMin = long.MinValue;
        private readonly long yMax = long.MaxValue;

        private Dictionary<long, HashSet<long>> aliveNodes = new();
        private HashSet<(long x, long y)> eligibleNodes = new();
        private List<UpdatedCell> UpdatesToApply = new();

        public DictionaryBoardSegment()
        {
        }

        public void Initialize(IReadOnlyList<UpdatedCell> updatesToApply)
        {
            // TODO: check that all updates are setting to alive
            this.ApplyUpdates(updatesToApply);
        }

        internal void FindEligibleNodes()
        {
            this.eligibleNodes = new();
            foreach(var aliveColumn in this.aliveNodes)
            {
                foreach (var aliveYPosition in aliveColumn.Value)
                {
                    //TODO: Deal with boundaries

                    // Iterate through the surrounding nodes and add them all to the nodes we need to check for state changes
                    // We rely on HashSet.Add to deal with duplicates
                    for (var xCoordinate = aliveColumn.Key - 1; xCoordinate <= aliveColumn.Key + 1; xCoordinate++)
                    {
                        for (var yCoordinate = aliveYPosition - 1; yCoordinate <= aliveYPosition + 1; yCoordinate++)
                        {
                            this.eligibleNodes.Add((xCoordinate, yCoordinate));
                        }
                    }
                }
            }
        }

        internal void CalculateNextState()
        {
            this.UpdatesToApply = new();
            foreach(var eligibleNode in this.eligibleNodes)
            {
                if(this.ShouldNodeUpdate(eligibleNode.x, eligibleNode.y, out var update))
                {
                    this.UpdatesToApply.Add(update);
                }
            }
        }

        internal void ApplyUpdates()
        {
            this.ApplyUpdates(this.UpdatesToApply);
        }

        private void ApplyUpdates(IReadOnlyList<UpdatedCell> updatesToApply)
        {
            foreach (var update in updatesToApply)
            {
                if(!this.aliveNodes.TryGetValue(update.xPosition, out var yNodes))
                {
                    yNodes = new HashSet<long>();
                    this.aliveNodes[update.xPosition] = yNodes;
                }

                if (update.isAlive)
                {
                    yNodes.Add(update.yPosition);
                }
                else
                {
                    yNodes.Remove(update.yPosition);
                    if (yNodes.Count == 0)
                    {
                        this.aliveNodes.Remove(update.xPosition);
                    }
                }
            }
        }

        /// <summary>
        /// If an "alive" cell had less than 2 or more than 3 alive neighbors (in any of the 8 surrounding cells), it becomes dead.
        /// If a "dead" cell had *exactly* 3 alive neighbors, it becomes alive.
        /// </summary>
        internal bool ShouldNodeUpdate(long x, long y, [NotNullWhen(true)] out UpdatedCell? update)
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
                        update = new UpdatedCell(x, y, false);
                        return true;
                    }
                }
            }

            // Check if we're currently a dead cell and have exactly 3 alive neighbors
            // or if we're alive and have less than 2 alive neighbors.
            // We don't need to check for currently alive and more than 3 as we would have already returned.
            if (!currentlyAlive && aliveCount == 3)
            {
                update = new UpdatedCell(x, y, true);
                return true;
            }

            if (currentlyAlive && aliveCount < 2)
            {
                update = new UpdatedCell(x, y, false);
                return true;
            }

            update = null;
            return false;
        }

        internal bool GetState(long x, long y)
        {
            /*
            if (x < xPosition || y < yPosition)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (x > xPosition + size || y > yPosition + size)
            {
                throw new ArgumentOutOfRangeException();
            }
            */
            //return this.state is not null && state[x - xPosition, y - yPosition];
            return false;
        }

        private bool IsNodeAlive(long x, long y)
        {
            if (x < this.xMin)
            {
                if (y < this.yMin)
                {
                    // Check corner segment
                    return false;
                }

                if (y >= this.yMax)
                {
                    // Check corner segment
                    return false;
                }

                // Check other segment
                return false;
            }

            if (x >= this.xMax)
            {
                if (y < this.yMin)
                {
                    // Check corner segment
                    return false;
                }

                if (y >= this.yMax)
                {
                    // Check corner segment
                    return false;
                }

                // Check other segment
                return false;
            }

            if (y < this.yMin)
            {
                // Check other segment
                return false;
            }

            if (y >= this.yMax)
            {
                // Check other segment
                return false;
            }

            try
            {
                return this.aliveNodes.TryGetValue(x, out var yValues) && yValues.Contains(y);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{x}, {y}: {ex}");
                return false;
            }
        }
    }
}
