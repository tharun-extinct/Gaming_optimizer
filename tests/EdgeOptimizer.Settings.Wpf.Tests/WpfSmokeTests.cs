using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using EdgeOptimizer.Settings.Wpf.Services;
using EdgeOptimizer.Settings.Wpf.ViewModels;
using EdgeOptimizer.Settings.Wpf.Views;

namespace EdgeOptimizer.Settings.Wpf.Tests;

public sealed class WpfSmokeTests
{
    [Fact]
    public void ApplicationResourcesWindowAndViewsLoadOnStaThread()
    {
        // Verifies WPF resources, views, window bounds, and automation metadata load without showing a window.
        RunOnSta(() =>
        {
            var application = Application.Current as App ?? new App();
            application.InitializeComponent();
            var viewModel = new MainWindowViewModel(new FakeFilePicker(null), new FakeRunnerClient());
            var window = new MainWindow { DataContext = viewModel };
            var views = new UserControl[] { new DashboardView(), new CrosshairView(), new MacrosView(), new SystemTweaksView() };

            Assert.Equal(1280d, window.MinWidth);
            Assert.Equal(760d, window.MinHeight);
            Assert.All(views, view => Assert.NotNull(view.Content));
            Assert.NotNull(window.FindName("ProfilesList"));
            Assert.NotNull(window.FindName("PageHost"));
            window.ApplyTemplate();

            var controls = Descendants(window).OfType<FrameworkElement>()
                .Where(element => !string.IsNullOrWhiteSpace(AutomationProperties.GetAutomationId(element)))
                .ToList();
            Assert.Contains(controls, element => AutomationProperties.GetAutomationId(element) == "Navigation.Dashboard");
            Assert.Contains(controls, element => AutomationProperties.GetAutomationId(element) == "Profiles.List");
            Assert.Contains(controls, element => AutomationProperties.GetAutomationId(element) == "Profile.Activate");
            Assert.All(controls, element => Assert.False(string.IsNullOrWhiteSpace(AutomationProperties.GetName(element))));
        });
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            yield return child;
            foreach (var descendant in Descendants(child)) yield return descendant;
        }
    }

    private static void RunOnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
