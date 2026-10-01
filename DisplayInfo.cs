namespace LiveDesktop;

sealed record DisplayInfo(string DeviceName, Rectangle Bounds, bool Primary)
{
    public static DisplayInfo[] Detect() => Screen.AllScreens
        .Select(s => new DisplayInfo(s.DeviceName, s.Bounds, s.Primary))
        .OrderByDescending(s => s.Primary).ThenBy(s => s.Bounds.X).ThenBy(s => s.Bounds.Y).ToArray();

    public static void Verify()
    {
        foreach (int count in new[] { 1, 2, 3, 8, 32 })
        {
            var displays = Enumerable.Range(0, count)
                .Select(i => new DisplayInfo($"TEST-{i}", new Rectangle((i - 1) * 1920, i % 2 * -1080, 1920, 1080), i == 1 || count == 1)).ToArray();
            using var controller = new Controller(() => displays);
            if (controller.DetectedMonitorCount != count) throw new Exception($"Expected {count} monitor cards.");
            displays = displays.Take(Math.Max(1, count / 2)).ToArray();
            controller.RebuildDisplays(false);
            if (controller.DetectedMonitorCount != displays.Length) throw new Exception("Monitor removal was not detected.");
        }
    }
}
