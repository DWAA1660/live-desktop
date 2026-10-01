using Microsoft.Win32;

namespace LiveDesktop;

static class StartupRegistration
{
    const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    const string Name = "LiveDesktop";
    public static string Command => $"\"{Environment.ProcessPath}\" --startup";
    public static bool Enabled
    {
        get { using var key = Registry.CurrentUser.OpenSubKey(RunKey); return string.Equals(key?.GetValue(Name) as string, Command, StringComparison.OrdinalIgnoreCase); }
    }
    public static void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKey);
        if (enabled) key.SetValue(Name, Command, RegistryValueKind.String);
        else key.DeleteValue(Name, false);
    }
}
