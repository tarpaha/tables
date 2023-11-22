namespace Solver.Result;

public record Player(int Id);
public record Table(int Id, IEnumerable<Player> Players);
public record Round(int Id, IEnumerable<Table> Tables);
public record Result(IEnumerable<Round> Rounds);