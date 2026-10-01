using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace LiveDesktop;

class Canvas : Control
{
    readonly System.Windows.Forms.Timer timer = new();
    readonly Stopwatch clock = Stopwatch.StartNew();
    double last, time;
    public double SceneTime => time;
    public event EventHandler? FrameUpdated;
    public int Theme { get; set; }
    public float Speed { get; set; } = 1;
    public bool Paused { get; set; }
    public Canvas()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        timer.Interval = 33; timer.Tick += (_, _) => { double now = clock.Elapsed.TotalSeconds; if (!Paused) time += Math.Min(now - last, .1) * Speed; last = now; if (!Paused) { FrameUpdated?.Invoke(this, EventArgs.Empty); if (Visible) Invalidate(); } }; timer.Start();
    }
    public void SetFps(int fps) => timer.Interval = 1000 / Math.Clamp(fps, 15, 60);
    public void Restart(double initialTime = 0) { time = initialTime; last = clock.Elapsed.TotalSeconds; Invalidate(); }
    protected override void Dispose(bool disposing) { if (disposing) timer.Dispose(); base.Dispose(disposing); }
    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); SceneRenderer.Draw(e.Graphics, ClientSize, Theme, time); }
}

static class SceneRenderer
{
    static void Fill(Graphics g, Color c, float x, float y, float w, float h) { using var b = new SolidBrush(c); g.FillEllipse(b, x, y, w, h); }
    public static void Draw(Graphics g, Size size, int theme, double t)
    {
        if (size.Width <= 0 || size.Height <= 0) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        if (theme == 0) { Rain(g, size, t); return; }
        var state = g.Save(); g.ScaleTransform(size.Width / 1600f, size.Height / 900f);
        if (theme <= 2) River(g, t, theme == 2); else ExtraScenes.Draw(g, theme, t);
        g.Restore(state);
    }
    static void Rain(Graphics g, Size size, double t)
    {
        g.Clear(Color.FromArgb(2, 9, 6)); g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        float cell = Math.Max(13, size.Width / 95f); using var font = new Font("Consolas", cell * .8f, FontStyle.Regular, GraphicsUnit.Pixel);
        const string symbols = "01ABCDEFGHIJKLMNOPQRSTUVWXYZ<>[]{}+=*/:アカサタナハマヤラワ";
        int cols = (int)(size.Width / cell) + 1;
        using var brush = new SolidBrush(Color.Lime);
        for (int col = 0; col < cols; col++)
        {
            var random = new Random(col * 7919 + 41); float velocity = 50 + random.Next(120); int length = 9 + random.Next(25);
            double span = size.Height + length * cell + 400;
            float head = (float)((t * velocity + random.NextDouble() * span) % span) - 100;
            for (int i = 0; i < length; i++)
            {
                float y = head - i * cell; if (y < -cell || y > size.Height) continue;
                int seed = unchecked(col * 173 + i * 127 + (int)(t * (i == 0 ? 13 : 3)) * 37);
                char ch = symbols[(seed & int.MaxValue) % symbols.Length];
                int alpha = (int)(245 * Math.Pow(1 - (double)i / length, .85));
                brush.Color = i == 0 ? Color.FromArgb(245, 194, 255, 212) : Color.FromArgb(alpha, 12, 230, 87);
                g.DrawString(ch.ToString(), font, brush, col * cell, y);
            }
        }
        using var shade = new LinearGradientBrush(new Rectangle(0, 0, size.Width, size.Height), Color.FromArgb(5, 0, 0, 0), Color.FromArgb(90, 0, 0, 0), 90f);
        g.FillRectangle(shade, 0, 0, size.Width, size.Height);
    }
    static void River(Graphics g, double t, bool night, bool highlights = true)
    {
        using var sky = new LinearGradientBrush(new Rectangle(0, 0, 1600, 900), night ? Color.FromArgb(15, 27, 63) : Color.FromArgb(134, 213, 226), night ? Color.FromArgb(63, 89, 126) : Color.FromArgb(250, 231, 191), 90f);
        g.FillRectangle(sky, 0, 0, 1600, 900);
        if (night)
        {
            var random = new Random(76);
            for (int i = 0; i < 70; i++) { float x = random.Next(1600), y = random.Next(390); int a = (int)(130 + 100 * Math.Sin(t * .7 + i)); Fill(g, Color.FromArgb(a, 247, 243, 210), x, y, 3, 3); }
            Fill(g, Color.FromArgb(35, 244, 245, 223), 1180, 85, 174, 174); Fill(g, Color.FromArgb(247, 246, 215), 1200, 105, 134, 134);
            Fill(g, Color.FromArgb(202, 210, 192), 1225, 135, 22, 22); Fill(g, Color.FromArgb(218, 222, 197), 1280, 178, 32, 32);
        }
        else
        {
            Fill(g, Color.FromArgb(35, 255, 247, 184), 1160, 70, 220, 220); Fill(g, Color.FromArgb(255, 237, 169), 1200, 110, 140, 140);
            for (int i = 0; i < 4; i++)
            {
                float x = (float)((i * 440 + t * (8 + i)) % 1900) - 220; float y = 130 + i % 2 * 120;
                var cloud = Color.FromArgb(145, 255, 253, 237); Fill(g, cloud, x, y, 190, 47); Fill(g, cloud, x + 40, y - 23, 73, 72); Fill(g, cloud, x + 95, y - 13, 68, 60);
            }
        }
        Hills(g, night ? Color.FromArgb(50, 79, 105) : Color.FromArgb(126, 179, 160), 400, 75, 0);
        Hills(g, night ? Color.FromArgb(37, 68, 87) : Color.FromArgb(94, 152, 131), 460, 45, 2);
        Waves(g, night ? Color.FromArgb(45, 111, 143) : Color.FromArgb(68, 170, 180), 480, 15, t * .7, 0);
        Waves(g, night ? Color.FromArgb(30, 88, 127) : Color.FromArgb(43, 150, 172), 560, 24, t, 1);
        Waves(g, night ? Color.FromArgb(25, 72, 111) : Color.FromArgb(30, 131, 157), 735, 24, t * 1.2, 2);
        Waves(g, night ? Color.FromArgb(20, 54, 89) : Color.FromArgb(23, 110, 143), 845, 19, t * 1.4, 3);
        using var foam = new Pen(night ? Color.FromArgb(75, 173, 219, 226) : Color.FromArgb(110, 209, 251, 237), 3);
        for (int row = 0; highlights && row < 4; row++) for (int i = 0; i < 9; i++)
        {
            float x = (float)((i * 210 + t * (16 + row * 8) + row * 72) % 1750) - 90;
            float y = 525 + row * 105 + (float)Math.Sin(x / 100 + t) * 9;
            g.DrawArc(foam, x, y, 48 + i % 3 * 15, 12, 8, 160);
        }
        // Finish all water and foam before the raft and beaver. The highlights
        // belong to the water, so they must never paint over the character.
        float bx = 800 + (float)Math.Sin(t * .20) * 300;
        float by = 582 + (float)Math.Sin(t * 1.6) * 15;
        var state = g.Save(); g.TranslateTransform(bx, by); g.RotateTransform((float)Math.Sin(t * 1.6 + .4) * 4);
        Beaver(g, t, night); g.Restore(state);
    }
    internal static void VerifyBeaverForeground()
    {
        foreach (bool night in new[] { false, true }) foreach (double time in new[] { 3.5, 18.0 })
        {
            using var withFoam = new Bitmap(1600, 900); using var withoutFoam = new Bitmap(1600, 900); using var mask = new Bitmap(1600, 900);
            using (var g = Graphics.FromImage(withFoam)) { g.SmoothingMode = SmoothingMode.AntiAlias; River(g, time, night, true); }
            using (var g = Graphics.FromImage(withoutFoam)) { g.SmoothingMode = SmoothingMode.AntiAlias; River(g, time, night, false); }
            using (var g = Graphics.FromImage(mask))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(800 + (float)Math.Sin(time * .20) * 300, 582 + (float)Math.Sin(time * 1.6) * 15);
                g.RotateTransform((float)Math.Sin(time * 1.6 + .4) * 4); Beaver(g, time, night);
            }
            int checkedPixels = 0;
            for (int x = 0; x < 1600; x += 3) for (int y = 300; y < 710; y += 3)
                if (mask.GetPixel(x, y).A == 255)
                {
                    checkedPixels++;
                    if (withFoam.GetPixel(x, y) != withoutFoam.GetPixel(x, y)) throw new Exception("Wave highlights overlap the beaver or raft.");
                }
            if (checkedPixels < 100) throw new Exception("Beaver foreground check did not sample the character.");
        }
    }
    static void Hills(Graphics g, Color c, float baseline, float amp, double phase)
    {
        using var path = new GraphicsPath(); path.AddLine(-10, 910, -10, baseline);
        for (int x = -10; x <= 1610; x += 10) path.AddLine(x, baseline + (float)Math.Sin(x / 220d + phase) * amp, x + 10, baseline + (float)Math.Sin((x + 10) / 220d + phase) * amp);
        path.AddLine(1610, baseline, 1610, 910); path.CloseFigure(); using var b = new SolidBrush(c); g.FillPath(b, path);
    }
    static void Waves(Graphics g, Color c, float baseline, float amp, double t, double phase)
    {
        using var path = new GraphicsPath(); path.AddLine(-10, 920, -10, baseline);
        for (int x = -10; x <= 1610; x += 10) { float Y(int at) => baseline + (float)(Math.Sin(at / 105d + t + phase) * amp + Math.Sin(at / 250d - t * .6) * amp * .4); path.AddLine(x, Y(x), x + 10, Y(x + 10)); }
        path.AddLine(1620, baseline, 1620, 920); path.CloseFigure(); using var b = new SolidBrush(c); g.FillPath(b, path);
    }
    static void Beaver(Graphics g, double t, bool night)
    {
        Fill(g, Color.FromArgb(35, 5, 45, 64), -190, 48, 390, 32);
        // A little driftwood raft, tied together with rope.
        using var log = new SolidBrush(Color.FromArgb(119, 74, 45)); using var grain = new Pen(Color.FromArgb(75, 47, 35), 3);
        for (int i = 0; i < 3; i++) { float y = 23 + i * 13; g.FillRectangle(log, -146, y, 290, 18); Fill(g, Color.FromArgb(179, 121, 72), -158, y, 26, 18); Fill(g, Color.FromArgb(179, 121, 72), 132, y, 26, 18); g.DrawLine(grain, -120, y + 7, 129, y + 8); }
        using var rope = new Pen(Color.FromArgb(227, 194, 130), 5); g.DrawLine(rope, -91, 24, -91, 65); g.DrawLine(rope, 88, 24, 88, 65);
        var brown = Color.FromArgb(147, 92, 57); var dark = Color.FromArgb(93, 58, 42); var light = Color.FromArgb(198, 142, 87);
        var state = g.Save(); g.TranslateTransform(94, 6); g.RotateTransform(-27);
        Fill(g, dark, -10, -9, 100, 51);
        using var tailPen = new Pen(Color.FromArgb(120, 78, 52), 2);
        var tailClip = g.Save(); using var tp = new GraphicsPath(); tp.AddEllipse(-10, -9, 100, 51); g.SetClip(tp);
        for (int i = -20; i < 120; i += 14) { g.DrawLine(tailPen, i, -12, i + 50, 50); g.DrawLine(tailPen, i, 50, i + 50, -12); } g.Restore(tailClip); g.Restore(state);
        Fill(g, brown, -78, -133, 159, 174); Fill(g, light, -49, -85, 100, 120);
        Fill(g, dark, -74, -199, 54, 54); Fill(g, dark, 27, -199, 54, 54);
        Fill(g, Color.FromArgb(207, 149, 117), -61, -188, 29, 30); Fill(g, Color.FromArgb(207, 149, 117), 39, -188, 29, 30);
        Fill(g, brown, -84, -190, 170, 135);
        Fill(g, Color.FromArgb(170, 110, 66), -74, -183, 145, 107);
        // Blinking eyes, rounded muzzle and the unmistakable front teeth.
        bool blink = t % 6.2 > 5.98;
        if (blink) { using var eyePen = new Pen(Color.FromArgb(40, 31, 30), 5); g.DrawArc(eyePen, -43, -147, 24, 9, 0, 180); g.DrawArc(eyePen, 23, -147, 24, 9, 0, 180); }
        else { Fill(g, Color.FromArgb(37, 31, 30), -42, -150, 19, 24); Fill(g, Color.FromArgb(37, 31, 30), 24, -150, 19, 24); Fill(g, Color.White, -38, -146, 6, 7); Fill(g, Color.White, 28, -146, 6, 7); }
        Fill(g, Color.FromArgb(225, 169, 130), -67, -125, 28, 17); Fill(g, Color.FromArgb(225, 169, 130), 42, -125, 28, 17);
        Fill(g, Color.FromArgb(224, 180, 129), -43, -124, 48, 40); Fill(g, Color.FromArgb(224, 180, 129), -1, -124, 48, 40);
        Fill(g, dark, -16, -128, 34, 23);
        using var smile = new Pen(dark, 3); g.DrawArc(smile, -25, -113, 52, 29, 15, 150);
        using var tooth = new SolidBrush(Color.FromArgb(255, 247, 220)); g.FillRectangle(tooth, -14, -91, 13, 21); g.FillRectangle(tooth, 2, -91, 13, 21);
        Fill(g, dark, -65, 10, 48, 28); Fill(g, dark, 24, 10, 48, 28);
        Fill(g, brown, -96, -67, 42, 72); Fill(g, brown, 59, -67, 42, 72);
        // Leaf held like a tiny flag.
        using var stem = new Pen(Color.FromArgb(76, 105, 57), 4); g.DrawLine(stem, 88, -20, 121, -106);
        var leafState = g.Save(); g.TranslateTransform(121, -105); g.RotateTransform(-30 + (float)Math.Sin(t * 2) * 8);
        Fill(g, Color.FromArgb(104, 157, 70), -8, -48, 34, 55); using var vein = new Pen(Color.FromArgb(177, 209, 110), 2); g.DrawLine(vein, 8, 0, 8, -38); g.Restore(leafState);
        if (night) { using var tint = new SolidBrush(Color.FromArgb(24, 32, 58, 108)); g.FillEllipse(tint, -100, -205, 220, 260); }
    }
}

static class SelfTest
{
    public static void Run(bool checkDesktop = true)
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "test-results"); Directory.CreateDirectory(folder);
        var lines = new List<string>();
        try
        {
            using var gallery = new Bitmap(1600, ((ThemeCatalog.Names.Length + 3) / 4) * 255);
            using var galleryGraphics = Graphics.FromImage(gallery);
            galleryGraphics.Clear(Color.FromArgb(16, 23, 34));
            using var caption = new Font("Segoe UI", 10, FontStyle.Bold);
            for (int theme = 0; theme < ThemeCatalog.Names.Length; theme++)
            {
                using var image = new Bitmap(1280, 720); using var g = Graphics.FromImage(image); SceneRenderer.Draw(g, image.Size, theme, 3.5); image.Save(Path.Combine(folder, $"scene-{theme}.png"));
                using var other = new Bitmap(1280, 720); using var og = Graphics.FromImage(other); SceneRenderer.Draw(og, other.Size, theme, 4.5);
                bool different = false; for (int x = 0; x < 1280 && !different; x += 11) for (int y = 0; y < 720; y += 11) if (image.GetPixel(x, y) != other.GetPixel(x, y)) { different = true; break; }
                if (!different) throw new Exception($"Theme {theme} did not animate."); lines.Add($"PASS: theme {theme} renders and changes over time.");
                int tileX = theme % 4 * 400, tileY = theme / 4 * 255;
                galleryGraphics.DrawImage(image, new Rectangle(tileX + 4, tileY + 4, 392, 221));
                galleryGraphics.DrawString(ThemeCatalog.Names[theme], caption, Brushes.White, tileX + 8, tileY + 230);
            }
            gallery.Save(Path.Combine(folder, "collection.png"));
            SceneRenderer.VerifyBeaverForeground(); lines.Add("PASS: water highlights stay behind the beaver and raft in both scenes.");
            RotationPicker.Verify(); lines.Add("PASS: random rotation stays within its pool, avoids immediate repeats, and uses different scenes per display.");
            DisplayInfo.Verify(); lines.Add("PASS: monitor controls and rotation support 1, 2, 3, 8 and 32 displays, including negative coordinates.");
            if (checkDesktop)
            {
            var host = Desktop.FindHost(); if (host == IntPtr.Zero) throw new Exception("No desktop host was found.");
            foreach (var screen in Screen.AllScreens)
            {
                using var w = new Wallpaper(screen.Bounds, 1); w.Show(); Desktop.Attach(w.Handle, host, screen.Bounds); w.StartDesktopRendering(); Application.DoEvents();
                if (Desktop.GetParent(w.Handle) != host) throw new Exception("Desktop attachment failed.");
                lines.Add($"PASS: desktop attachment on {screen.DeviceName}, {screen.Bounds}.");
                w.Scene.Paused = true;
                foreach (int nextTheme in new[] { 18, 19, 20 })
                {
                    w.Scene.Theme = nextTheme; w.Scene.Restart(47.2); w.RefreshScene(); Application.DoEvents();
                    if (Desktop.GetParent(w.Handle) != host) throw new Exception("Scene switching detached the wallpaper.");
                }
                lines.Add($"PASS: new animal scenes present and switch in place on {screen.DeviceName}, including while paused."); w.Close();
            }
            Desktop.Refresh(); lines.Add("PASS: all smoke checks complete.");
            }
            using var controller = new Controller();
            controller.Show(); Application.DoEvents();
            using var ui = new Bitmap(controller.Width, controller.Height);
            controller.DrawToBitmap(ui, new Rectangle(Point.Empty, controller.Size));
            ui.Save(Path.Combine(folder, "controls.png"));
            lines.Add("PASS: controls initialize and render with detected monitors.");
        }
        catch (Exception ex) { lines.Add("FAIL: " + ex); Environment.ExitCode = 1; }
        File.WriteAllLines(Path.Combine(folder, "results.txt"), lines);
    }
}
