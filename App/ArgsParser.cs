namespace App;

public static class ArgsParser
{
    public static string HelpText()
    {
        return "The program seats players at tables to play several rounds, neighbors are always new.\n" +
               "Format: tables.exe [players] [rounds] [3/4]\n" +
               "    [players] - total number of players\n" +
               "    [rounds]  - how much rounds to play\n" +
               "    [3/4]     - preferred table size, can be only 3 or 4\n" +
               "Example:\n" +
               "    tables.exe 37 3 4\n";
    }
    
    public static (int players, int rounds, bool prefer4) Parse(string[] args)
    {
        if (args.Length != 3)
            throw new ArgumentException("There must be 3 arguments");

        if (!int.TryParse(args[0], out var players))
            throw new ArgumentException("Cannot parse [players] parameter");

        if (!int.TryParse(args[1], out var rounds))
            throw new ArgumentException("Cannot parse [rounds] parameter");

        if (!int.TryParse(args[2], out var threeOrFour))
            throw new ArgumentException("Cannot parse [3/4] parameter");
        if (threeOrFour != 3 && threeOrFour != 4)
            throw new ArgumentException("[3/4] parameter must be 3 or 4");
    
        return (players, rounds, threeOrFour == 4);
    }
}