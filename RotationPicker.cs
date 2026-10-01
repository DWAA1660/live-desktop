namespace LiveDesktop;

sealed class RotationCountdown
{
    public double Seconds { get; private set; } = 1800;
    public void Reset(int minutes) => Seconds = Math.Max(1, minutes) * 60;
    public bool Advance(double elapsed, bool enabled, bool paused)
    {
        if (!enabled || paused) return false;
        Seconds -= Math.Max(0, elapsed);
        return Seconds <= 0;
    }
}

static class RotationPicker
{
    public static int[] Pool(int kind)
    {
        var animals = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 18, 19, 20 };
        return kind == 0 ? animals : kind == 1 ? animals.Concat(new[] { 14, 16, 17 }).ToArray() : Enumerable.Range(0, ThemeCatalog.Names.Length).ToArray();
    }
    public static int[] Next(int[] pool, int[] current, bool shuffle, Random? random = null)
    {
        random ??= Random.Shared;
        var next = new List<int>();
        foreach (int previous in current)
        {
            if (shuffle)
            {
                var available = pool.Where(t => !current.Contains(t) && !next.Contains(t)).ToArray();
                if (available.Length == 0) available = pool.Where(t => t != previous && !next.Contains(t)).ToArray();
                if (available.Length == 0) available = pool.Where(t => t != previous).ToArray();
                if (available.Length == 0) available = pool;
                next.Add(available[random.Next(available.Length)]);
            }
            else
            {
                int index = Array.IndexOf(pool, previous), choice = pool[0];
                for (int step = 1; step <= pool.Length; step++) { choice = pool[(index + step) % pool.Length]; if (!next.Contains(choice)) break; }
                next.Add(choice);
            }
        }
        return next.ToArray();
    }
    public static void Verify()
    {
        var timer = new RotationCountdown(); timer.Reset(30);
        if (timer.Advance(1799, true, false) || timer.Advance(3600, false, false) || timer.Advance(3600, true, true) || timer.Seconds != 1 || !timer.Advance(1, true, false)) throw new Exception("Rotation countdown or pause behavior failed.");
        foreach (int kind in new[] { 0, 1, 2 })
        {
            int[] pool = Pool(kind), current = { 1, 0 };
            var random = new Random(7919);
            for (int round = 0; round < 200; round++)
            {
                int[] next = Next(pool, current, true, random);
                if (next.Any(t => !pool.Contains(t)) || next.Distinct().Count() != 2 || next.Any(current.Contains)) throw new Exception("Random rotation repeated a current scene or selected outside its pool.");
                current = next;
            }
        }
        if (!Next(new[] { 3, 4, 5 }, new[] { 3, 4 }, false).SequenceEqual(new[] { 4, 5 })) throw new Exception("Ordered rotation did not advance.");
        foreach (int count in new[] { 1, 3, 8, 32 })
        foreach (int kind in new[] { 0, 1, 2 })
        {
            var pool = Pool(kind); var current = Enumerable.Range(0, count).Select(i => pool[i % pool.Length]).ToArray();
            for (int round = 0; round < 50; round++)
            {
                var next = Next(pool, current, true);
                if (next.Length != count || next.Any(t => !pool.Contains(t)) || next.Where((t, i) => t == current[i]).Any()) throw new Exception($"Rotation failed with {count} displays.");
                current = next;
            }
        }
    }
}
