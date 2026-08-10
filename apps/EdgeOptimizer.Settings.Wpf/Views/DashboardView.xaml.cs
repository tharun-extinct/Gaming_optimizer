using System.Windows;
using System.Windows.Controls;

namespace EdgeOptimizer.Settings.Wpf.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    public event EventHandler<string>? NavigateRequested;

    private void QuickAction_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string page })
        {
            NavigateRequested?.Invoke(this, page);
        }
    }
}
