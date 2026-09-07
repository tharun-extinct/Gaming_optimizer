using EdgeOptimizer.Settings.Core.ViewModels;
using EdgeOptimizer.Settings.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EdgeOptimizer.Settings.WinUI;

public sealed partial class ShellPage : Page
{
    private readonly MainWindowViewModel _viewModel;
    public UIElement DragRegion => TitleBarDragRegion;

    public ShellPage(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Navigation.SelectedItem = Navigation.MenuItems[0];
        Show("Dashboard");
    }

    private void OnNavigationChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer?.Tag is string target) Show(target);
    }

    private void Show(string target)
    {
        _viewModel.NavigateTo(target);
        FrameworkElement page = target switch { "Crosshair" => new CrosshairView(), "Macros" => new MacrosView(), "SystemTweaks" => new SystemTweaksView(), _ => new DashboardView() };
        page.DataContext = _viewModel.CurrentPage;
        PageHost.Content = page;
    }
}
