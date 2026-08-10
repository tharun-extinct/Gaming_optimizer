using System.Windows;
using EdgeOptimizer.Settings.Wpf.Services;
using EdgeOptimizer.Settings.Wpf.ViewModels;

namespace EdgeOptimizer.Settings.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var viewModel = new MainWindowViewModel(new WpfFilePicker(), new DisconnectedRunnerClient());
        MainWindow = new MainWindow { DataContext = viewModel };
        MainWindow.Show();
    }
}
