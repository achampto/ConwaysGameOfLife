using System.Collections.Generic;
using System.Linq;

namespace ConwaysGameOfLife
{
    internal class BoardState
    {
        /// <summary>
        /// The collection of cells that are alive.
        /// Key is the x value. Value is a hashset of all the alive y values for that x value.
        /// </summary>
        private readonly Dictionary<long, HashSet<long>> aliveCells = new();

        /// <summary>
        /// Initializes the board state with the cells that are alive to begin with.
        /// </summary>
        /// <param name="initialAliveCells">The list of cells that are initially alive.</param>
        public void Initialize(IReadOnlyList<(long x, long y)> initialAliveCells)
        {
            this.ApplyUpdates(initialAliveCells.Select(
                initialValue => new UpdatedCell(initialValue.x, initialValue.y, true)));
        }

        /// <summary>
        /// Progresses to the next generation. This will update the state of cells according to the following rules:
        /// If an "alive" cell had less than 2 or more than 3 alive neighbors (in any of the 8 surrounding cells), it becomes dead.
        /// If a "dead" cell had *exactly* 3 alive neighbors, it becomes alive.
        /// </summary>
        public void ProgressToNextGeneration()
        {
            var eligibleNodes = this.FindEligibleNodes();
            var updates = this.CalculateUpdates(eligibleNodes);
            this.ApplyUpdates(updates);
        }

        /// <summary>
        /// Retrieves the collection of cells that are currently alive.
        /// </summary>
        /// <returns>The alive cells.</returns>
        public IEnumerable<(long x, long y)> GetAliveCells()
        {
            foreach (var aliveColumn in this.aliveCells)
            {
                foreach (var aliveYPosition in aliveColumn.Value)
                {
                    yield return (aliveColumn.Key, aliveYPosition);
                }
            }
        }

        /// <summary>
        /// Finds all of the nodes that are eligible for a state change.
        /// </summary>
        /// <returns>The nodes that need to be examined to determine if they're state has changed.</returns>
        private IEnumerable<(long x, long y)> FindEligibleNodes()
        {
            var eligibleNodes = new HashSet<(long x, long y)>();
            foreach(var aliveColumn in this.aliveCells)
            {
                foreach (var aliveYPosition in aliveColumn.Value)
                {
                    // Iterate through the alive cells and add all of the cells surrounding an alive cell as we need to check those for state changes
                    // We rely on HashSet.Add to deal with duplicates
                    foreach (var xCoordinate in this.GetSurroundingValues(aliveColumn.Key))
                    {
                        foreach (var yCoordinate in this.GetSurroundingValues(aliveYPosition))
                        {
                            eligibleNodes.Add((xCoordinate, yCoordinate));
                        }
                    }
                }
            }

            return eligibleNodes;
        }

        /// <summary>
        /// Looks through the eligible nodes to determine if any updates need to occur as part of this generation.
        /// </summary>
        /// <param name="eligibleNodes">The eligible nodes to examine.</param>
        /// <returns>The cell updates which should occur.</returns>
        private IEnumerable<UpdatedCell> CalculateUpdates(IEnumerable<(long x, long y)> eligibleNodes)
        {
            var updates = new List<UpdatedCell>();
            foreach(var (x, y) in eligibleNodes)
            {
                var update = this.DetermineCellUpdate(x, y);

                if (update is not null)
                {
                    updates.Add(update);
                }
            }

            return updates;
        }

        /// <summary>
        /// Applies the cell updates to the board state.
        /// </summary>
        /// <param name="updatesToApply">The updates to apply.</param>
        private void ApplyUpdates(IEnumerable<UpdatedCell> updatesToApply)
        {
            foreach (var update in updatesToApply)
            {
                if(!this.aliveCells.TryGetValue(update.xPosition, out var yNodes))
                {
                    yNodes = new HashSet<long>();
                    this.aliveCells[update.xPosition] = yNodes;
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
                        this.aliveCells.Remove(update.xPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a particular cell should update according to:
        /// If an "alive" cell had less than 2 or more than 3 alive neighbors (in any of the 8 surrounding cells), it becomes dead.
        /// If a "dead" cell had *exactly* 3 alive neighbors, it becomes alive.
        /// </summary>
        /// <param name="x">The x position of the cell.</param>
        /// <param name="y">The y position of the cell.</param>
        /// <returns>The update to perform to the cell. Null if no updates should occur.</returns>
        private UpdatedCell? DetermineCellUpdate(long x, long y)
        {
            var currentlyAlive = this.IsCellAlive(x, y);
            var aliveCount = 0;

            // Iterate through the surrounding nodes and find which ones are alive.
            foreach (var xCoordinate in this.GetSurroundingValues(x))
            {
                foreach (var yCoordinate in this.GetSurroundingValues(y))
                {
                    // Don't check self
                    if (xCoordinate == x && yCoordinate == y)
                    {
                        continue;
                    }

                    // Check if the neighbor node is alive
                    if (this.IsCellAlive(xCoordinate, yCoordinate))
                    {
                        aliveCount++;
                    }

                    // Short circuit if we're currently alive and more than 3 neighbor nodes are alive
                    if (currentlyAlive && aliveCount > 3)
                    {
                        return new UpdatedCell(x, y, false);
                    }
                }
            }

            // Check if we're currently a dead cell and have exactly 3 alive neighbors
            if (!currentlyAlive && aliveCount == 3)
            {
                return new UpdatedCell(x, y, true);
            }

            // Check if we're alive and have less than 2 alive neighbors.
            if (currentlyAlive && aliveCount < 2)
            {
                return new UpdatedCell(x, y, false);
            }

            // No updates need to occur.
            return null;
        }

        /// <summary>
        /// Determines whether the cell at a particular location is alive.
        /// </summary>
        private bool IsCellAlive(long x, long y)
        {
            return this.aliveCells.TryGetValue(x, out var yValues) && yValues.Contains(y);
        }

        /// <summary>
        /// Returns the surrounding values for a number including range checking.
        /// </summary>
        private IEnumerable<long> GetSurroundingValues(long number)
        {
            return number switch
            {
                long.MaxValue => new[] { number - 1, number },
                long.MinValue => new[] { number, number + 1 },
                _ => new[] { number - 1, number, number + 1 },
            };
        }
    }
}
