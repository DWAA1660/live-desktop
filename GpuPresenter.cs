using System.Runtime.InteropServices;

namespace LiveDesktop;

sealed class GpuPresenter : IDisposable
{
    IntPtr swap, device, context, texture;
    [StructLayout(LayoutKind.Sequential)] struct Rational { public uint Numerator, Denominator; }
    [StructLayout(LayoutKind.Sequential)] struct Mode { public uint Width, Height; public Rational Refresh; public uint Format, Scanline, Scaling; }
    [StructLayout(LayoutKind.Sequential)] struct Samples { public uint Count, Quality; }
    [StructLayout(LayoutKind.Sequential)] struct Description { public Mode Mode; public Samples Samples; public uint Usage, Buffers; public IntPtr Window; public int Windowed; public uint Effect, Flags; }
    [DllImport("d3d11.dll")] static extern int D3D11CreateDeviceAndSwapChain(IntPtr adapter, uint driver, IntPtr software, uint flags, IntPtr levels, uint count, uint sdk, ref Description description, out IntPtr swap, out IntPtr device, out uint level, out IntPtr context);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int GetBuffer(IntPtr self, uint index, ref Guid iid, out IntPtr resource);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate void Upload(IntPtr self, IntPtr resource, uint index, IntPtr box, IntPtr data, uint pitch, uint depth);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] delegate int PresentFrame(IntPtr self, uint sync, uint flags);
    readonly Upload upload;
    readonly PresentFrame present;
    static T Method<T>(IntPtr com, int slot) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(Marshal.ReadIntPtr(Marshal.ReadIntPtr(com), slot * IntPtr.Size));
    public GpuPresenter(IntPtr hwnd, Size size)
    {
        var description = new Description { Mode = new Mode { Width = (uint)size.Width, Height = (uint)size.Height, Format = 87 }, Samples = new Samples { Count = 1 }, Usage = 0x20, Buffers = 1, Window = hwnd, Windowed = 1 };
        int result = D3D11CreateDeviceAndSwapChain(IntPtr.Zero, 1, IntPtr.Zero, 0x20, IntPtr.Zero, 0, 7, ref description, out swap, out device, out _, out context);
        if (result < 0) { Dispose(); throw new InvalidOperationException($"Could not initialize desktop graphics (0x{result:X8})."); }
        var iid = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
        result = Method<GetBuffer>(swap, 9)(swap, 0, ref iid, out texture);
        if (result < 0) { Dispose(); Marshal.ThrowExceptionForHR(result); }
        upload = Method<Upload>(context, 48); present = Method<PresentFrame>(swap, 8);
    }
    public void Draw(IntPtr pixels, int pitch)
    {
        upload(context, texture, 0, IntPtr.Zero, pixels, (uint)pitch, 0);
        int result = present(swap, 0, 0);
        if (result < 0) Marshal.ThrowExceptionForHR(result);
    }
    public void Dispose()
    {
        foreach (var p in new[] { texture, context, device, swap }) if (p != IntPtr.Zero) Marshal.Release(p);
        texture = context = device = swap = IntPtr.Zero;
    }
}
