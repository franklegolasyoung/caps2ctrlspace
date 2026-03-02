using System.ComponentModel;
using System.Windows;
using CapsMapper.Services;
using Wpf.Ui.Controls;

namespace CapsMapper.Views;

public partial class MainWindow : FluentWindow
{
    private readonly KeyboardHookService _hookService;
    private bool _isExiting;

    public MainWindow(KeyboardHookService hookService)
    {
        _hookService = hookService;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Watch for system theme changes
        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

        // Load auto-start state
        try
        {
            AutoStartToggle.IsChecked = AutoStartService.IsEnabled;
        }
        catch
        {
            AutoStartToggle.IsChecked = false;
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_isExiting)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            HideToTray();
            return;
        }

        TrayIcon.Dispose();
        base.OnClosing(e);
    }

    private void HideToTray()
    {
        Hide();
    }

    private void ShowFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void AutoStartToggle_Changed(object sender, RoutedEventArgs e)
    {
        try
        {
            AutoStartService.SetEnabled(AutoStartToggle.IsChecked == true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Failed to update auto-start:\n{ex.Message}",
                "Caps2CtrlSpace", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

            // Revert UI
            AutoStartToggle.Checked -= AutoStartToggle_Changed;
            AutoStartToggle.Unchecked -= AutoStartToggle_Changed;
            AutoStartToggle.IsChecked = !AutoStartToggle.IsChecked;
            AutoStartToggle.Checked += AutoStartToggle_Changed;
            AutoStartToggle.Unchecked += AutoStartToggle_Changed;
        }
    }

    private void TrayIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
    {
        // Single click - no action (keeps it unobtrusive)
    }

    private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowFromTray();
    }

    private void TrayMenu_Show_Click(object sender, RoutedEventArgs e)
    {
        ShowFromTray();
    }

    private void TrayMenu_Exit_Click(object sender, RoutedEventArgs e)
    {
        _isExiting = true;
        Application.Current.Shutdown();
    }
}
