using System.Windows;
using CapsMapper.Services;
using CapsMapper.Views;

namespace CapsMapper;

public partial class App : Application
{
    private SingleInstanceGuard? _guard;
    private KeyboardHookService? _hookService;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Single instance check
        _guard = new SingleInstanceGuard("Caps2CtrlSpace_SingleInstance");
        if (!_guard.IsFirstInstance)
        {
            MessageBox.Show("Caps2CtrlSpace is already running.", "Caps2CtrlSpace",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        // Install keyboard hook
        _hookService = new KeyboardHookService();
        try
        {
            _hookService.Install();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to install keyboard hook:\n{ex.Message}",
                "Caps2CtrlSpace Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        // Apply theme before window creation
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);

        // Create and show main window
        _mainWindow = new MainWindow(_hookService);
        _mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _hookService?.Dispose();
        _guard?.Dispose();
        base.OnExit(e);
    }
}
