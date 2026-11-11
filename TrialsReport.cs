static class TrialsReport
{
    public static void Run(string[] args)
    {
        if (args.Length != 2)
            throw new FormatException("Usage: trials study.tsv");
        var seen = new HashSet<string>();
        var plots = Rows(args[1], 5).Select(r =>
        {
            Require(seen.Add(r[0]), "Duplicate plot ID: " + r[0]);
            return (Id: r[0], Block: r[1], Treatment: r[2], Area: Positive(r[3]), Mass: Amount(r[4]));
        }).ToArray();
        var treatments = plots.Select(p => p.Treatment).Distinct().Order(StringComparer.Ordinal).ToArray();
        Require(treatments.Length == 2, "Provide exactly two treatments.");
        var differences = new List<decimal>();
        foreach (var block in plots.GroupBy(p => p.Block))
        {
            var yields = block.GroupBy(p => p.Treatment).ToDictionary(g => g.Key, g => 10000 * g.Sum(p => p.Mass) / g.Sum(p => p.Area));
            foreach (var p in block)
                Console.WriteLine($"{p.Id}\t{p.Block}\t{p.Treatment}\t{10000 * p.Mass / p.Area:0.##} kg/ha");
            if (yields.Count < 2)
            {
                Console.WriteLine($"{block.Key}: incomplete pair, missing {treatments.Single(t => !yields.ContainsKey(t))}");
                continue;
            }

            var delta = yields[treatments[1]] - yields[treatments[0]];
            differences.Add(delta);
            Console.WriteLine($"{block.Key}: {treatments[1]} minus {treatments[0]}, difference {delta:+0.##;-0.##;0} kg/ha");
        }

        if (differences.Count > 0)
            Console.WriteLine($"Mean within-block difference {differences.Average():0.##} kg/ha ({differences.Count} complete blocks)");
    }

    static List<string[]> Rows(string file, int width = 0, bool allowEmpty = false)
    {
        using var reader = file == "-" ? Console.In : File.OpenText(file);
        var rows = new List<string[]>();
        int line = 0;
        while (reader.ReadLine()is string s)
        {
            line++;
            if (string.IsNullOrWhiteSpace(s) || s.TrimStart().StartsWith("#"))
                continue;
            var cells = s.Split('\t', StringSplitOptions.TrimEntries);
            if ((width > 0 && cells.Length != width) || (!allowEmpty && cells.Any(string.IsNullOrEmpty)))
                throw new FormatException($"Invalid columns on line {line}.");
            rows.Add(cells);
        }

        if (rows.Count == 0)
            throw new FormatException("Empty input.");
        return rows;
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new FormatException(message);
    }

    static decimal Amount(string text)
    {
        var value = decimal.Parse(text, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint);
        Require(value >= 0, "Quantity must be nonnegative.");
        return value;
    }

    static decimal Positive(string text)
    {
        var value = Amount(text);
        Require(value > 0, "Quantity must be positive.");
        return value;
    }
}
