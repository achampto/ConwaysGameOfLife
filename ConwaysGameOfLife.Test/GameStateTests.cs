using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ConwaysGameOfLife.Test
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void TestArrayGameState()
        {
            var initialState = new List<(int, int)>
            {
                (0,1),
                (1,2),
                (2,0),
                (2,1),
                (2,2),
            };
            var boardSegment = new ArrayBoardSegment(0, 0, 2048);
            boardSegment.Initialize(initialState);
            
            for(int i = 0; i < 5; i++)
            {
                boardSegment.CalculateNextState();
                boardSegment.ApplyUpdates();

            }
        }

        [TestMethod]
        public void TestDictionaryGameState()
        {
            var initialState = new List<UpdatedCell>
            {
                new (0, 1, true),
                new (1, 2, true),
                new (2, 0, true),
                new (2, 1, true),
                new (2, 2, true),
            };
            var boardSegment = new DictionaryBoardSegment();
            boardSegment.Initialize(initialState);

            for (int i = 0; i < 25; i++)
            {
                boardSegment.FindEligibleNodes();
                boardSegment.CalculateNextState();
                boardSegment.ApplyUpdates();
            }

            Console.WriteLine("finished");
        }
    }
}