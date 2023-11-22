namespace Solver;

public static class Divider
{
    public static int[] DivideByTables(int personsCount, bool prefer4)
    {
        if (personsCount is <= 0 or 1 or 2 or 5)
        {
            throw new ArgumentException($"Cannot divide {personsCount} persons by tables of 3 or 4");
        }

        var result = new List<int>();

        if (prefer4)
        {
            while (personsCount % 4 != 0)
            {
                personsCount -= 3;
                result.Add(3);
            }
            for (var i = 0; i < personsCount / 4; i++)
            {
                result.Add(4);
            }
            return result.OrderDescending().ToArray();
        }
        else
        {
            while (personsCount % 3 != 0)
            {
                personsCount -= 4;
                result.Add(4);
            }
            for (var i = 0; i < personsCount / 3; i++)
            {
                result.Add(3);
            }
            return result.Order().ToArray();
        }
    }
}