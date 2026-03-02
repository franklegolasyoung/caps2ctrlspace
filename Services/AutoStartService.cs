using Microsoft.Win32;

namespace CapsMapper.Services;

/// <summary>
/// Manages auto-start registration via Windows Registry.
/// </summary>
public static class AutoStartService
{
    private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Caps2CtrlSpace";

    public static bool IsEnabled
    {
        get
        {
            try
            {
                using var rk = Registry.CurrentUser.OpenSubKey(RunKey, false);
                var value = rk?.GetValue(AppName)?.ToString();
                return value != null && string.Equals(value, GetExePath(), StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
    }

    public static void SetEnabled(bool enabled)
    {
        using var rk = Registry.CurrentUser.OpenSubKey(RunKey, true)
            ?? throw new InvalidOperationException("Cannot open Run registry key.");

        if (enabled)
            rk.SetValue(AppName, GetExePath());
        else
            rk.DeleteValue(AppName, throwOnMissingValue: false);
    }

    private static string GetExePath()
    {
        return Environment.ProcessPath ?? AppDomain.CurrentDomain.BaseDirectory + "Caps2CtrlSpace.exe";
    }
}
