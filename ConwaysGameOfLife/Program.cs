using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ConwaysGameOfLife
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (TryParseInputs(args, out var initialState))
            {
                // Establish an event handler to process key press events.
                Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

                var boardState = new BoardState();
                boardState.Initialize(initialState);

                Console.WriteLine("Initial State: ");
                PrintAliveCells(boardState.GetAliveCells());

                Console.WriteLine("Beginning program in 3 seconds. Press Ctrl+C to exit.");
                await Task.Delay(3000);

                while(true)
                {
                    boardState.ProgressToNextGeneration();
                    PrintAliveCells(boardState.GetAliveCells());
                }
            }
        }

        static bool TryParseInputs(string[] args, [NotNullWhen(true)] out List<(long x, long y)>? initialState)
        {
            try
            {
                initialState = new List<(long x, long y)>();
                for (int i = 0; i < args.Length; i = i + 2)
                {
                    // Parse X & Y values
                    var x = long.Parse(args[i].Trim('(', ','));
                    var y = long.Parse(args[i + 1].Trim(',', ')'));
                    initialState.Add((x, y));
                }

                return true;
            }
            catch
            {
                Console.WriteLine("Invalid input. Expect initial state to be in the form of '(x, y)'");
                initialState = null;
                return false;
            }
        }

        static void PrintAliveCells(IEnumerable<(long x, long y)> aliveCells)
        {
            Console.WriteLine(string.Join(", ", aliveCells));
        }

        protected static void CancelHandler(object? sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Ending the program.");
            Environment.Exit(0);
        }
    }
}