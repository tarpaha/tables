namespace App;
using Args;
using Solver;

public static class Program
{
    private const int MaxWorkDurationMs = 5000;
    
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var (players, rounds, prefer4) = Parser.Parse(args);
            var cts = new CancellationTokenSource(); cts.CancelAfter(MaxWorkDurationMs);
            var result = await Solver.Solve(players, rounds, prefer4, cts.Token);
            ResultWriter.WriteToConsole(result, rounds);
        }
        catch (ArgumentException ex)
        {
            await Console.Out.WriteLineAsync(HelpText());
            await Console.Error.WriteLineAsync($"Error: {ex.Message}");
            return 1;
        }
        catch (OperationCanceledException)
        {
            await Console.Error.WriteLineAsync($"Error: No solution found after {MaxWorkDurationMs}ms");
            return 1;
        }
        return 0;
    }

    private static string HelpText()
    {
        return "The program seats players at tables to play several rounds, neighbors are always new.\n" +
               "Format: tables.exe [players] [rounds] [3/4]\n" +
               "    [players] - total number of players\n" +
               "    [rounds]  - how much rounds to play\n" +
               "    [3/4]     - preferred table size, can be only 3 or 4\n" +
               "Example:\n" +
               "    tables.exe 37 3 4\n";
    }
}