using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LiveDesktop;

// A persistent, top-down DIB shared by GDI+ and the desktop compositor.
// This avoids relying on WM_PAINT redirection beneath Explorer's no-redirection host.
sealed class LayeredSurface : IDisposable
{
    readonly IntPtr dc, dib, previous;
    readonly Bitmap bitmap;
    readonly Graphics graphics;
    readonly IntPtr pixels;
    readonly GpuPresenter gpu;
    [StructLayout(LayoutKind.Sequential)] struct Header
    {
        public uint Size; public int Width, Height; public ushort Planes, BitCount;
        public uint Compression, ImageSize; public int XPels, YPels; public uint ColorsUsed, ColorsImportant;
    }
    [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleDC(IntPtr dc);
    [DllImport("gdi32.dll")] static extern IntPtr CreateDIBSection(IntPtr dc, ref Header info, uint usage, out IntPtr bits, IntPtr section, uint offset);
    [DllImport("gdi32.dll")] static extern IntPtr SelectObject(IntPtr dc, IntPtr obj);
    [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr obj);
    [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr dc);
    public LayeredSurface(Form window)
    {
        int width = window.ClientSize.Width, height = window.ClientSize.Height;
        dc = CreateCompatibleDC(IntPtr.Zero);
        var header = new Header { Size = 40, Width = width, Height = -height, Planes = 1, BitCount = 32, ImageSize = (uint)(width * height * 4) };
        dib = CreateDIBSection(dc, ref header, 0, out var bits, IntPtr.Zero, 0);
        pixels = bits;
        if (dc == IntPtr.Zero || dib == IntPtr.Zero) { if (dib != IntPtr.Zero) DeleteObject(dib); if (dc != IntPtr.Zero) DeleteDC(dc); throw new InvalidOperationException("Could not allocate the desktop animation surface."); }
        previous = SelectObject(dc, dib);
        bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bits);
        graphics = Graphics.FromImage(bitmap);
        try { gpu = new GpuPresenter(window.Handle, bitmap.Size); }
        catch { graphics.Dispose(); bitmap.Dispose(); SelectObject(dc, previous); DeleteObject(dib); DeleteDC(dc); throw; }
    }
    public void Present(int theme, double time)
    {
        SceneRenderer.Draw(graphics, bitmap.Size, theme, time); graphics.Flush();
        gpu.Draw(pixels, bitmap.Width * 4);
    }
    public void Dispose() { gpu.Dispose(); graphics.Dispose(); bitmap.Dispose(); SelectObject(dc, previous); DeleteObject(dib); DeleteDC(dc); }
}
