namespace App;
using Solver;

public static class Program
{
    private const int MaxWorkDurationMs = 5000;
    
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var (players, rounds, prefer4) = ArgsParser.Parse(args);
            var cts = new CancellationTokenSource(); cts.CancelAfter(MaxWorkDurationMs);
            var result = await Solver.Solve(players, rounds, prefer4, cts.Token);
            ResultWriter.WriteToConsole(result, rounds);
        }
        catch (ArgumentException ex)
        {
            await Console.Out.WriteLineAsync(ArgsParser.HelpText());
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
}