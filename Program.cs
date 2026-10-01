using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace LiveDesktop;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Contains("--self-test")) { SelfTest.Run(); return; }
        if (args.Contains("--render-test")) { SelfTest.Run(false); return; }
        using var mutex = new Mutex(true, "Local\\LiveDesktop.BeaverAndRain", out bool first);
        if (!first) { if (!args.Contains("--startup")) MessageBox.Show("Live Desktop is already running. Open it from the system tray."); return; }
        var controller = new Controller();
        if (args.Contains("--startup")) controller.Shown += (_, _) => controller.StartAutomatic();
        else if (args.Contains("--apply")) controller.Shown += (_, _) => controller.Apply();
        Application.Run(controller);
    }
}

class Preferences
{
    public Dictionary<string, int> Themes { get; set; } = new();
    public int Speed { get; set; } = 100;
    public int Fps { get; set; } = 30;
    public bool Rotate { get; set; } = true;
    public int RotationMinutes { get; set; } = 30;
    public int RotationPool { get; set; } = 1;
    public bool Shuffle { get; set; } = true;
    public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LiveDesktop");
    public static Preferences Load() { try { return JsonSerializer.Deserialize<Preferences>(File.ReadAllText(Path.Combine(Folder, "settings.json"))) ?? new(); } catch { return new(); } }
    public void Save() { Directory.CreateDirectory(Folder); File.WriteAllText(Path.Combine(Folder, "settings.json"), JsonSerializer.Serialize(this)); }
}

class Controller : Form
{
    readonly Preferences prefs = Preferences.Load();
    readonly List<Wallpaper> wallpapers = new();
    readonly List<(DisplayInfo screen, ComboBox choice, Canvas preview)> monitors = new();
    readonly Func<DisplayInfo[]> detectDisplays;
    public int DetectedMonitorCount => monitors.Count;
    readonly FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(16) };
    readonly Label status = new() { AutoSize = true, ForeColor = Color.LightSteelBlue };
    readonly TrackBar speed = new() { Minimum = 25, Maximum = 200, TickFrequency = 25, Width = 180 };
    readonly ComboBox fps = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80, ForeColor = Color.Black, BackColor = Color.White };
    readonly NotifyIcon tray;
    readonly CheckBox rotate = new() { Text = "Rotate backgrounds", AutoSize = true, ForeColor = Color.White, Margin = new Padding(0, 10, 8, 0) };
    readonly ComboBox interval = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100, ForeColor = Color.Black, BackColor = Color.White };
    readonly ComboBox pool = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 155, ForeColor = Color.Black, BackColor = Color.White };
    readonly ComboBox order = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100, ForeColor = Color.Black, BackColor = Color.White };
    readonly System.Windows.Forms.Timer rotationTimer = new() { Interval = 1000 };
    readonly System.Windows.Forms.Timer displayTimer = new() { Interval = 750 };
    readonly System.Windows.Forms.Timer startupTimer = new() { Interval = 3000 };
    readonly CheckBox startWithWindows = new() { Text = "Start with Windows", AutoSize = true, ForeColor = Color.White, Margin = new Padding(14, 10, 0, 0) };
    int startupAttempts;
    DateTime lastRotationTick = DateTime.UtcNow;
    readonly RotationCountdown rotationCountdown = new();
    bool active, paused, exiting;
    public Controller(Func<DisplayInfo[]>? displayProvider = null)
    {
        detectDisplays = displayProvider ?? DisplayInfo.Detect;
        Text = "Live Desktop • Rain & River"; Size = new Size(900, 720); MinimumSize = new Size(860, 670);
        StartPosition = FormStartPosition.CenterScreen; BackColor = Color.FromArgb(16, 23, 34); ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);
        var header = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(24) };
        header.Controls.Add(new Label { Text = "Make your desktop move.", AutoSize = true, Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(24, 12) });
        header.Controls.Add(new Label { Text = "Choose a scene for each display. Your desktop icons stay usable.", AutoSize = true, ForeColor = Color.LightSteelBlue, Location = new Point(28, 65) });
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 210, Padding = new Padding(20), WrapContents = true };
        var apply = Button("Apply wallpapers", () => Apply());
        AcceptButton = apply;
        bottom.Controls.Add(apply);
        bottom.Controls.Add(Button("Pause / Resume", () => { paused = !paused; foreach (var w in wallpapers) w.Scene.Paused = paused; UpdateStatus(); }));
        bottom.Controls.Add(Button("Restore desktop", Stop));
        bottom.Controls.Add(Button("Hide to tray", Hide));
        bottom.SetFlowBreak(bottom.Controls[^1], true);
        bottom.Controls.Add(new Label { Text = "Motion speed", AutoSize = true, Margin = new Padding(0, 13, 0, 0) });
        speed.Value = Math.Clamp(prefs.Speed, 25, 200); bottom.Controls.Add(speed);
        fps.Items.AddRange(new object[] { "15 fps", "30 fps", "60 fps" }); fps.SelectedIndex = prefs.Fps == 15 ? 0 : prefs.Fps == 60 ? 2 : 1;
        bottom.Controls.Add(fps);
        bottom.Controls.Add(Button("Refresh displays", () => RebuildDisplays()));
        startWithWindows.Checked = StartupRegistration.Enabled;
        bottom.Controls.Add(startWithWindows);
        startWithWindows.CheckedChanged += (_, _) =>
        {
            try { StartupRegistration.SetEnabled(startWithWindows.Checked); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not change startup", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        };
        bottom.SetFlowBreak(bottom.Controls[^1], true);
        rotate.Checked = prefs.Rotate;
        interval.Items.AddRange(new object[] { "15 min", "30 min", "1 hour", "2 hours", "4 hours" });
        int[] intervals = { 15, 30, 60, 120, 240 }; interval.SelectedIndex = Math.Max(0, Array.IndexOf(intervals, prefs.RotationMinutes));
        pool.Items.AddRange(new object[] { "Cute animals", "Cute + calm", "All backgrounds" }); pool.SelectedIndex = Math.Clamp(prefs.RotationPool, 0, 2);
        order.Items.AddRange(new object[] { "Random", "In order" }); order.SelectedIndex = prefs.Shuffle ? 0 : 1;
        bottom.Controls.Add(rotate); bottom.Controls.Add(interval); bottom.Controls.Add(pool); bottom.Controls.Add(order);
        bottom.Controls.Add(Button("Next background", RotateNow));
        bottom.SetFlowBreak(bottom.Controls[^1], true); bottom.Controls.Add(status);
        Controls.Add(cards); Controls.Add(bottom); Controls.Add(header);
        speed.ValueChanged += (_, _) => Tune(); fps.SelectedIndexChanged += (_, _) => Tune();
        tray = new NotifyIcon { Icon = SystemIcons.Application, Text = "Live Desktop", Visible = true };
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open controls", null, (_, _) => { Show(); WindowState = FormWindowState.Normal; Activate(); });
        menu.Items.Add("Pause / Resume", null, (_, _) => { paused = !paused; foreach (var w in wallpapers) w.Scene.Paused = paused; UpdateStatus(); });
        menu.Items.Add("Restore desktop", null, (_, _) => Stop());
        menu.Items.Add("Next background", null, (_, _) => RotateNow());
        menu.Items.Add("Exit", null, (_, _) => { exiting = true; Close(); }); tray.ContextMenuStrip = menu;
        tray.DoubleClick += (_, _) => { Show(); Activate(); };
        FormClosing += (_, e) => { if (!exiting && e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; Hide(); tray.ShowBalloonTip(2500, "Live Desktop", "Still running. Right-click the tray icon to restore your desktop or exit.", ToolTipIcon.Info); } };
        Microsoft.Win32.SystemEvents.DisplaySettingsChanged += DisplayChanged;
        BuildCards(); UpdateStatus();
        rotationCountdown.Reset(prefs.RotationMinutes);
        rotate.CheckedChanged += (_, _) => RotationChanged(); interval.SelectedIndexChanged += (_, _) => RotationChanged();
        pool.SelectedIndexChanged += (_, _) => RotationChanged(); order.SelectedIndexChanged += (_, _) => RotationChanged();
        rotationTimer.Tick += (_, _) =>
        {
            var now = DateTime.UtcNow; double elapsed = Math.Max(0, (now - lastRotationTick).TotalSeconds); lastRotationTick = now;
            if (active && rotationCountdown.Advance(elapsed, prefs.Rotate, paused)) RotateNow();
            UpdateStatus();
        };
        rotationTimer.Start();
        displayTimer.Tick += (_, _) => { displayTimer.Stop(); RebuildDisplays(); };
        startupTimer.Tick += (_, _) =>
        {
            startupAttempts++; Apply(false);
            if (active || startupAttempts >= 20) startupTimer.Stop();
        };
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= DisplayChanged;
            rotationTimer.Dispose(); displayTimer.Dispose(); startupTimer.Dispose();
            Stop(); tray.Dispose();
        }
        base.Dispose(disposing);
    }
    public void StartAutomatic() { Hide(); startupAttempts = 0; startupTimer.Start(); }
    void RotationChanged()
    {
        prefs.Rotate = rotate.Checked; prefs.RotationMinutes = new[] { 15, 30, 60, 120, 240 }[interval.SelectedIndex];
        prefs.RotationPool = pool.SelectedIndex; prefs.Shuffle = order.SelectedIndex == 0;
        rotationCountdown.Reset(prefs.RotationMinutes); lastRotationTick = DateTime.UtcNow; prefs.Save(); UpdateStatus();
    }
    void RotateNow()
    {
        if (!active || wallpapers.Count != monitors.Count) return;
        int[] themes = RotationPicker.Next(RotationPicker.Pool(prefs.RotationPool), wallpapers.Select(w => w.Scene.Theme).ToArray(), prefs.Shuffle);
        for (int i = 0; i < themes.Length; i++)
        {
            monitors[i].choice.SelectedIndex = themes[i]; wallpapers[i].Scene.Theme = themes[i];
            wallpapers[i].Scene.Restart(Random.Shared.NextDouble() * 120); wallpapers[i].RefreshScene(); prefs.Themes[monitors[i].screen.DeviceName] = themes[i];
        }
        rotationCountdown.Reset(prefs.RotationMinutes); lastRotationTick = DateTime.UtcNow; prefs.Save(); UpdateStatus();
    }
    void DisplayChanged(object? sender, EventArgs e)
    {
        if (!IsDisposed && IsHandleCreated) BeginInvoke((Action)(() => { if (!IsDisposed) { displayTimer.Stop(); displayTimer.Start(); } }));
    }
    internal void RebuildDisplays(bool persist = true)
    {
        bool restart = active, wasPaused = paused;
        foreach (var m in monitors) prefs.Themes[m.screen.DeviceName] = m.choice.SelectedIndex;
        if (persist) prefs.Save(); Stop(); BuildCards();
        if (restart) { Apply(false); paused = wasPaused; foreach (var w in wallpapers) w.Scene.Paused = paused; }
        UpdateStatus();
    }
    Button Button(string text, Action action) { var b = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(35, 53, 71), ForeColor = Color.White, Padding = new Padding(8, 4, 8, 4) }; b.Click += (_, _) => action(); return b; }
    void BuildCards()
    {
        foreach (var m in monitors) m.preview.Dispose(); monitors.Clear();
        while (cards.Controls.Count > 0) { var c = cards.Controls[0]; cards.Controls.Remove(c); c.Dispose(); }
        int n = 0;
        foreach (var screen in detectDisplays())
        {
            n++; var panel = new Panel { Width = 390, Height = 300, Margin = new Padding(8), BackColor = Color.FromArgb(24, 34, 47) };
            var choice = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(16, 52), Width = 356, ForeColor = Color.Black, BackColor = Color.White };
            choice.Items.AddRange(ThemeCatalog.Names);
            choice.MaxDropDownItems = 12; choice.IntegralHeight = false; choice.DropDownHeight = 300;
            choice.SelectedIndex = prefs.Themes.TryGetValue(screen.DeviceName, out int theme) ? Math.Clamp(theme, 0, ThemeCatalog.Names.Length - 1) : (n - 1) % 2;
            panel.Controls.Add(new Label { Text = $"DISPLAY {n}{(screen.Primary ? " • PRIMARY" : "")}   {screen.Bounds.Width} × {screen.Bounds.Height}", AutoSize = true, Location = new Point(16, 18) });
            var preview = new Canvas { Location = new Point(16, 94), Size = new Size(356, 190), Theme = choice.SelectedIndex };
            choice.SelectedIndexChanged += (_, _) => { preview.Theme = choice.SelectedIndex; };
            panel.Controls.Add(choice); panel.Controls.Add(preview); cards.Controls.Add(panel); monitors.Add((screen, choice, preview));
        }
        Tune();
    }
    void Tune()
    {
        prefs.Speed = speed.Value; prefs.Fps = fps.SelectedIndex == 0 ? 15 : fps.SelectedIndex == 2 ? 60 : 30;
        foreach (var m in monitors) { m.preview.Speed = prefs.Speed / 100f; m.preview.SetFps(prefs.Fps); }
        foreach (var w in wallpapers) { w.Scene.Speed = prefs.Speed / 100f; w.Scene.SetFps(prefs.Fps); }
    }
    public void Apply(bool showErrors = true)
    {
        Stop();
        try
        {
            var host = Desktop.FindHost();
            if (host == IntPtr.Zero) throw new InvalidOperationException("Windows did not expose a desktop wallpaper layer. Restart Windows Explorer and try again.");
            foreach (var m in monitors)
            {
                var w = new Wallpaper(m.screen.Bounds, m.choice.SelectedIndex); wallpapers.Add(w); w.Show(); Desktop.Attach(w.Handle, host, m.screen.Bounds); w.StartDesktopRendering();
                prefs.Themes[m.screen.DeviceName] = m.choice.SelectedIndex;
            }
            active = true; paused = false; rotationCountdown.Reset(prefs.RotationMinutes); lastRotationTick = DateTime.UtcNow; Tune(); prefs.Save(); UpdateStatus();
        }
        catch (Exception ex)
        {
            Stop();
            if (showErrors) MessageBox.Show(this, ex.Message, "Could not apply wallpaper", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else { status.Text = "Waiting for the Windows desktop • " + ex.Message; Directory.CreateDirectory(Preferences.Folder); File.WriteAllText(Path.Combine(Preferences.Folder, "startup-error.log"), ex.ToString()); }
        }
    }
    void Stop() { foreach (var w in wallpapers) { w.Close(); w.Dispose(); } wallpapers.Clear(); active = false; paused = false; Desktop.Refresh(); UpdateStatus(); }
    void UpdateStatus() { status.Text = active ? $"{(paused ? "Paused" : "Live")} on {wallpapers.Count} displays{(prefs.Rotate ? $" • next scene in {Math.Max(1, (int)Math.Ceiling(rotationCountdown.Seconds / 60))} min" : "")} • close to keep running in the tray" : $"Detected {monitors.Count} displays • select your scenes, then click Apply wallpapers"; }
}

class Wallpaper : Form
{
    public Canvas Scene { get; }
    LayeredSurface? layered;
    public Wallpaper(Rectangle bounds, int theme)
    {
        FormBorderStyle = FormBorderStyle.None; ShowInTaskbar = false; StartPosition = FormStartPosition.Manual; Bounds = bounds;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Scene = new Canvas { Theme = theme };
        Scene.FrameUpdated += (_, _) => { if (!IsDisposed && IsHandleCreated) { if (layered != null) layered.Present(Scene.Theme, Scene.SceneTime); else Invalidate(); } };
    }
    public void StartDesktopRendering() { if (Desktop.UseLayeredHost) { Desktop.PrepareExplicitLayer(Handle); layered = new LayeredSurface(this); layered.Present(Scene.Theme, Scene.SceneTime); } }
    public void RefreshScene() { if (layered != null) layered.Present(Scene.Theme, Scene.SceneTime); else Invalidate(); }
    protected override void OnPaint(PaintEventArgs e) { if (layered == null) SceneRenderer.Draw(e.Graphics, ClientSize, Scene.Theme, Scene.SceneTime); }
    protected override void Dispose(bool disposing) { if (disposing) { Scene.Dispose(); layered?.Dispose(); layered = null; } base.Dispose(disposing); }
    protected override bool ShowWithoutActivation => true;
    protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ExStyle |= 0x08000000 | 0x80; if (Desktop.UseLayeredHost) cp.ExStyle |= 0x00080000; return cp; } }
}

static class Desktop
{
    static IntPtr shellView, wallpaperWorker;
    static bool raisedDesktop;
    public static bool UseLayeredHost => raisedDesktop;
    public static void PrepareExplicitLayer(IntPtr hwnd)
    {
        int style = GetWindowLong(hwnd, -20);
        SetWindowLong(hwnd, -20, style & ~0x00080000);
        SetWindowLong(hwnd, -20, style | 0x00080000);
        if (!SetLayeredWindowAttributes(hwnd, 0, 255, 2)) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
    }
    public delegate bool EnumProc(IntPtr hwnd, IntPtr parameter);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern IntPtr FindWindow(string cls, string? name);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern IntPtr FindWindowEx(IntPtr parent, IntPtr after, string cls, string? name);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc callback, IntPtr param);
    [DllImport("user32.dll")] static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr w, IntPtr l, uint flags, uint timeout, out IntPtr result);
    [DllImport("user32.dll", SetLastError = true)] static extern IntPtr SetParent(IntPtr child, IntPtr parent);
    [DllImport("user32.dll")] public static extern IntPtr GetParent(IntPtr child);
    [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr hwnd, int index);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hwnd, int index, int value);
    [DllImport("user32.dll", SetLastError = true)] static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll")] static extern bool MapWindowPoints(IntPtr from, IntPtr to, ref Point point, uint count);
    [DllImport("user32.dll")] static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool erase);
    [DllImport("user32.dll", SetLastError = true)] static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint colorKey, byte alpha, uint flags);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] static extern bool RedrawWindow(IntPtr hwnd, IntPtr rect, IntPtr region, uint flags);
    public static IntPtr FindHost()
    {
        var progman = FindWindow("Progman", null); if (progman == IntPtr.Zero) return IntPtr.Zero;
        SendMessageTimeout(progman, 0x052C, new IntPtr(0xD), IntPtr.Zero, 2, 1000, out _);
        SendMessageTimeout(progman, 0x052C, new IntPtr(0xD), new IntPtr(1), 2, 1000, out _);
        shellView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
        wallpaperWorker = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
        raisedDesktop = (GetWindowLong(progman, -20) & 0x00200000) != 0 && shellView != IntPtr.Zero;
        // Windows 11's raised desktop needs a layered sibling ABOVE the stock
        // wallpaper WorkerW, not a child buried inside that wallpaper window.
        if (raisedDesktop) return progman;
        IntPtr host = IntPtr.Zero;
        EnumWindows((window, _) => { if (FindWindowEx(window, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero) host = FindWindowEx(IntPtr.Zero, window, "WorkerW", null); return true; }, IntPtr.Zero);
        // Recent Windows 11 Explorer builds place the wallpaper WorkerW inside Progman.
        if (host == IntPtr.Zero) host = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
        return host;
    }
    public static void Attach(IntPtr child, IntPtr host, Rectangle bounds)
    {
        SetWindowLong(child, -16, (GetWindowLong(child, -16) & ~unchecked((int)0x80000000)) | 0x40000000);
        if (raisedDesktop)
        {
            // Reset the layer after initial WinForms creation. The Direct3D
            // presenter initializes its opaque composition surface after attachment.
            SetWindowLong(child, -20, GetWindowLong(child, -20) & ~0x00080000);
            SetWindowLong(child, -20, GetWindowLong(child, -20) | 0x00080000);
        }
        SetParent(child, host);
        if (GetParent(child) != host) throw new InvalidOperationException("Unable to attach animation to the Windows desktop.");
        var origin = bounds.Location; MapWindowPoints(IntPtr.Zero, host, ref origin, 1);
        if (!SetWindowPos(child, raisedDesktop && host != wallpaperWorker ? shellView : IntPtr.Zero, origin.X, origin.Y, bounds.Width, bounds.Height, 0x0010 | 0x0040 | 0x0020)) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        if (raisedDesktop && wallpaperWorker != IntPtr.Zero)
            SetWindowPos(wallpaperWorker, new IntPtr(1), 0, 0, 0, 0, 0x0010 | 0x0001 | 0x0002);
        if (!IsWindowVisible(child)) throw new InvalidOperationException("Windows attached the wallpaper to an invisible desktop layer.");
        RedrawWindow(child, IntPtr.Zero, IntPtr.Zero, 0x0001 | 0x0100 | 0x0080);
        if (raisedDesktop) RedrawWindow(shellView, IntPtr.Zero, IntPtr.Zero, 0x0001 | 0x0004 | 0x0100 | 0x0080);
    }
    public static void Refresh() { var p = FindWindow("Progman", null); if (p != IntPtr.Zero) InvalidateRect(p, IntPtr.Zero, true); }
}
