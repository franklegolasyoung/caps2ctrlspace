using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CapsMapper.Services;

/// <summary>
/// Low-level keyboard hook that intercepts CapsLock and sends Ctrl+Space instead.
/// Based on the original Caps2CtrlSpace project by cuiliang.
/// </summary>
public sealed class KeyboardHookService : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    private const byte VK_CAPITAL = 0x14;
    private const byte VK_CONTROL = 0x11;
    private const byte VK_SPACE = 0x20;
    private const byte VK_SHIFT = 0x10;
    private const byte VK_MENU = 0x12;
    private const byte VK_LWIN = 0x5B;
    private const byte VK_RWIN = 0x5C;

    private const uint KEYEVENTF_KEYUP = 0x0002;

    private readonly LowLevelKeyboardProc _hookProc;
    private nint _hookId;
    private bool _disposed;

    public event Action? KeyMapped;
    public bool IsHooked => _hookId != 0;

    public KeyboardHookService()
    {
        _hookProc = HookCallback;
    }

    public void Install()
    {
        if (_hookId != 0) return;

        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule
            ?? throw new InvalidOperationException("Cannot access main module.");

        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);

        if (_hookId == 0)
        {
            var error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to install keyboard hook. Error: {error}");
        }
    }

    public void Uninstall()
    {
        if (_hookId != 0)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = 0;
        }
    }

    private nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
        {
            var vkCode = Marshal.ReadInt32(lParam);

            if (vkCode == VK_CAPITAL && !AnyModifierPressed())
            {
                SendCtrlSpace();
                KeyMapped?.Invoke();
                return 1; // Suppress original CapsLock
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static bool AnyModifierPressed()
    {
        return (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0
            || (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0
            || (GetAsyncKeyState(VK_MENU) & 0x8000) != 0
            || (GetAsyncKeyState(VK_LWIN) & 0x8000) != 0
            || (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0;
    }

    private static void SendCtrlSpace()
    {
        keybd_event(VK_CONTROL, 0, 0, 0);
        keybd_event(VK_SPACE, 0, 0, 0);
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, 0);
    }

    public void Dispose()
    {
        if (!_disposed) { Uninstall(); _disposed = true; }
    }

    #region Native Interop

    private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    #endregion
}
