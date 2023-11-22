namespace App;
using Solver.Result;

public static class ResultWriter
{
    public static void WriteToConsole(Result result, int rounds)
    {
        foreach (var round in result.Rounds)
        {
            Console.WriteLine($"Round: {round.Id}");
            foreach (var table in round.Tables)
            {
                var players = string.Join(", ", table.Players.Select(p => p.Id.ToString("D2")));
                Console.WriteLine($"{table.Id:D2} - {players}");
            }
            Console.WriteLine();
        }
        
        if(rounds > result.Rounds.Count())
            Console.Error.WriteLine($"Cannot find solution for {rounds} rounds");
    }
}