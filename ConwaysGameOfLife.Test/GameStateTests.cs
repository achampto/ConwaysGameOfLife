using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ConwaysGameOfLife.Test
{
    [TestClass, TestCategory("BVT")]
    public class GameStateTests
    {
        [TestMethod]
        public void GameStateProperlyInitializes()
        {
            var initialState = new List<(long x, long y)>
            {
                (0, 1),
                (1, 2),
                (2, 0),
                (2, 1),
                (2, 2),
            };
            var boardState = new BoardState();
            boardState.Initialize(initialState);
            var aliveCells = boardState.GetAliveCells();
            aliveCells.Should().BeEquivalentTo(initialState);
        }

        [TestMethod]
        public void GameStateProgressAsExpected()
        {
            var initialState = new List<(long x, long y)>
            {
                (0, 1),
                (1, 2),
                (2, 0),
                (2, 1),
                (2, 2),
            };
            var boardState = new BoardState();
            boardState.Initialize(initialState);

            var expectedFirstGenerationState = new List<(long x, long y)>
            {
                // (0, 1) dies
                (1, 0), // newly alive
                (1, 2), // remains
                // (2, 0) dies
                (2, 1), // remains
                (2, 2), // remains
                (3, 1), // newly alive
            };

            boardState.ProgressToNextGeneration();
            var actualFirstGeneration = boardState.GetAliveCells();
            actualFirstGeneration.Should().BeEquivalentTo(expectedFirstGenerationState);

            var expectedSecondGenerationState = new List<(long x, long y)>
            {
                // (1, 0) dies
                (1, 2), // remains
                (2, 0), // newly alive
                // (2, 1) dies
                (2, 2), //remains
                (3, 1), // remains
                (3, 2), // newly alive
            };

            boardState.ProgressToNextGeneration();
            var actualSecondGeneration = boardState.GetAliveCells();
            actualSecondGeneration.Should().BeEquivalentTo(expectedSecondGenerationState);
        }

        [TestMethod]
        public void GameStateHandlesStableBlocksNearEdges()
        {
            var initialState = new List<(long x, long y)>
            {
                // Create a stable block with an X value near long.MaxValue
                (long.MaxValue, 1),
                (long.MaxValue - 1, 1),
                (long.MaxValue, 2),
                (long.MaxValue - 1, 2),

                // Create a stable block with an Y value near long.MaxValue
                (1, long.MaxValue),
                (1, long.MaxValue - 1),
                (2, long.MaxValue),
                (2, long.MaxValue - 1),

                // Create a stable block with an X value near long.MaxValue
                (long.MaxValue, long.MaxValue),
                (long.MaxValue, long.MaxValue - 1 ),
                (long.MaxValue - 1, long.MaxValue),
                (long.MaxValue - 1, long.MaxValue - 1),

                // Create a stable block with an X value near long.MinValue
                (long.MinValue, 1),
                (long.MinValue + 1, 1),
                (long.MinValue, 2),
                (long.MinValue + 1, 2),

                // Create a stable block with an Y value near long.MinValue
                (1, long.MinValue),
                (1, long.MinValue + 1),
                (2, long.MinValue),
                (2, long.MinValue + 1),

                // Create a stable block with X & Y value near long.MinValue
                (long.MinValue, long.MinValue),
                (long.MinValue, long.MinValue + 1),
                (long.MinValue + 1, long.MinValue),
                (long.MinValue + 1, long.MinValue + 1),
            };

            var boardState = new BoardState();
            boardState.Initialize(initialState);
            var actualInitializedState = boardState.GetAliveCells();
            actualInitializedState.Should().BeEquivalentTo(initialState);

            // Verify the configuration is stable for a few generations
            for (int i = 0; i < 5; i++)
            {
                boardState.ProgressToNextGeneration();
                var aliveCells = boardState.GetAliveCells();
                aliveCells.Should().BeEquivalentTo(initialState);
            }
        }

        [TestMethod]
        public void GameStateHandlesDyingCellsNearTheEdges()
        {
            var initialState = new List<(long x, long y)>
            {
                // Create a stable block with an X value near long.MaxValue
                (long.MaxValue, 1),
                (long.MaxValue, 2),

                // Create a stable block with an Y value near long.MaxValue
                (1, long.MaxValue),
                (2, long.MaxValue),

                // Create a stable block with an X value near long.MaxValue
                (long.MaxValue, long.MaxValue),
                (long.MaxValue, long.MaxValue - 1 ),

                // Create a stable block with an X value near long.MinValue
                (long.MinValue, 1),
                (long.MinValue, 2),

                // Create a stable block with an Y value near long.MinValue
                (1, long.MinValue),
                (2, long.MinValue),

                // Create a stable block with X & Y value near long.MinValue
                (long.MinValue, long.MinValue),
                (long.MinValue + 1, long.MinValue),
            };

            var boardState = new BoardState();
            boardState.Initialize(initialState);
            var actualInitializedState = boardState.GetAliveCells();
            actualInitializedState.Should().BeEquivalentTo(initialState);

            boardState.ProgressToNextGeneration();
            var actualFirstGeneration = boardState.GetAliveCells();
            actualFirstGeneration.Should().BeEquivalentTo(Array.Empty<(long x, long y)>());
        }
    }
}