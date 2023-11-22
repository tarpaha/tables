namespace Solver;

public class Player
{
    private readonly HashSet<Player> _neighbours = new();
    public IEnumerable<Player> Neighbours => _neighbours;
    public bool HadNeighbour(Player player) => _neighbours.Contains(player);
    public void AddNeighbour(Player player) { _neighbours.Add(player); }

    public int Id { get; }
    public Player(int id) => Id = id;
    public override string ToString() => Id.ToString("D2");
}