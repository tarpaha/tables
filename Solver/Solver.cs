namespace Solver;

public static class Solver
{
    private const int ShuffleCount = 100000;

    public static async Task<Result.Result> Solve(int playersCount, int roundsCount, bool prefer4, CancellationToken ct)
    {
        return await Task.Run(() => ProcessSolve(playersCount, roundsCount, prefer4, ct), ct);
    }

    private static Result.Result ProcessSolve(int playersCount, int roundsCount, bool prefer4, CancellationToken ct)
    {
        if(playersCount <= 0)
            throw new ArgumentException("Players count parameter must be greater than zero");
        if(roundsCount <= 0)
            throw new ArgumentException("Rounds count parameter must be greater than zero");
        
        var rnd = new Random(DateTime.Now.Millisecond);
        var players = Enumerable.Range(1, playersCount).Select(id => new Player(id)).ToList();

        var tablesCapacity = Divider.DivideByTables(playersCount, prefer4);

        var rounds = new List<Result.Round>();
        for (var roundId = 0; roundId < roundsCount; roundId++)
        {
            var s = 0;
            for (; s < ShuffleCount; s++)
            {
                var playersCopy = CopyPlayers(players);
                var availablePlayers = Shuffle(playersCopy, rnd);
                
                var tables = new List<Result.Table>();
                var tableId = 0;
                for (; tableId < tablesCapacity.Length; tableId++)
                {
                    var tableCapacity = tablesCapacity[tableId];
                    var neighbours = GetNonNeighbourPlayers(new List<Player>(), availablePlayers, tableCapacity, ct);
                    if (neighbours.Count != tableCapacity)
                        break;
                    
                    availablePlayers = availablePlayers.Except(neighbours).ToList();
                    for(var i = 0; i < neighbours.Count; i++)
                    for (var j = i + 1; j < neighbours.Count; j++)
                    {
                        neighbours[i].AddNeighbour(neighbours[j]);
                        neighbours[j].AddNeighbour(neighbours[i]);
                    }
                    
                    tables.Add(new Result.Table(tableId + 1, neighbours.Select(n => new Result.Player(n.Id))));
                }
                if (tableId == tablesCapacity.Length)
                {
                    rounds.Add(new Result.Round(roundId + 1, tables));
                    players = playersCopy;
                    break;
                }
            }

            if (s == ShuffleCount)
            {
                break;
            }
        }

        return new Result.Result(rounds);
    }
    
    private static List<Player> CopyPlayers(List<Player> players)
    {
        var newPlayersDict = players.ToDictionary(p => p.Id, p => new Player(p.Id));
        foreach (var player in players)
        {
            var newPlayer = newPlayersDict[player.Id];
            foreach (var neighbour in player.Neighbours)
            {
                newPlayer.AddNeighbour(newPlayersDict[neighbour.Id]);
            }
        }
        return newPlayersDict.Values.ToList();
    }
    
    private static List<Player> Shuffle(IEnumerable<Player> players, Random rnd)
    {
        return players
            .Select(p => (p, rnd.Next()))
            .OrderBy(pair => pair.Item2)
            .Select(pair => pair.Item1)
            .ToList();
    }
    
    private static List<Player> GetNonNeighbourPlayers(
        List<Player> alreadyChosenPlayers, List<Player> availablePlayers, int count,
        CancellationToken ct)
    {
        foreach (var player in availablePlayers)
        {
            ct.ThrowIfCancellationRequested();
            if (alreadyChosenPlayers.Any(alreadyChosenPlayer => alreadyChosenPlayer.HadNeighbour(player)))
                continue;
            var playerList = new List<Player> { player };
            if (count == 1)
                return playerList;
            var neighbours = GetNonNeighbourPlayers(
                alreadyChosenPlayers.Append(player).ToList(),
                availablePlayers.Where(p => p != player).ToList(),
                count - 1, ct);
            var result = playerList.Concat(neighbours).ToList();
            if(result.Count == count)
                return result;
        }
        return new List<Player>();
    }
}