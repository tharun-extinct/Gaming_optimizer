using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using EdgeOptimizer.Settings.Wpf.Models;
using EdgeOptimizer.Settings.Wpf.Views;

namespace EdgeOptimizer.Settings.Wpf;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly DashboardView _dashboardView = new();
    private readonly CrosshairView _crosshairView = new();
    private readonly MacrosView _macrosView = new();
    private readonly SystemTweaksView _systemTweaksView = new();

    private ProfileSummary? _selectedProfile;
    private string _currentPageLabel = "Dashboard";
    private string _currentPageTitle = "Gaming dashboard";
    private string _statusMessage = "Design preview loaded. Commands remain disabled until Runner IPC is connected.";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        Profiles.Add(new ProfileSummary("Fortnite", true));
        Profiles.Add(new ProfileSummary("Valorant", false));
        Profiles.Add(new ProfileSummary("Shadow of Tomb Raider", false));
        SelectedProfile = Profiles[0];
        ProfilesList.SelectedIndex = 0;

        _dashboardView.NavigateRequested += NavigateTo;
        ShowPage("Dashboard");
    }

    public ObservableCollection<ProfileSummary> Profiles { get; } = new();

    public ProfileSummary? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (ReferenceEquals(_selectedProfile, value))
            {
                return;
            }

            _selectedProfile = value;
            OnPropertyChanged();
            StatusMessage = value is null
                ? "Select a profile to continue."
                : $"Loaded {value.Name} preview state. No durable data was changed.";
        }
    }

    public string CurrentPageLabel
    {
        get => _currentPageLabel;
        private set => SetField(ref _currentPageLabel, value);
    }

    public string CurrentPageTitle
    {
        get => _currentPageTitle;
        private set => SetField(ref _currentPageTitle, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Navigation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string page })
        {
            ShowPage(page);
        }
    }

    private void ShowPage(string page)
    {
        switch (page)
        {
            case "Crosshair":
                PageHost.Content = _crosshairView;
                CurrentPageLabel = "Crosshair";
                CurrentPageTitle = "Crosshair overlay";
                break;
            case "Macros":
                PageHost.Content = _macrosView;
                CurrentPageLabel = "Macros";
                CurrentPageTitle = "Macro editor";
                break;
            case "SystemTweaks":
                PageHost.Content = _systemTweaksView;
                CurrentPageLabel = "System Tweaks";
                CurrentPageTitle = "System tweaks";
                break;
            default:
                DashboardNav.IsChecked = true;
                PageHost.Content = _dashboardView;
                CurrentPageLabel = "Dashboard";
                CurrentPageTitle = "Gaming dashboard";
                break;
        }

        StatusMessage = $"Showing {CurrentPageLabel} for {SelectedProfile?.Name ?? "the selected profile"}. Preview data is memory-only.";
    }

    private void NavigateTo(object? sender, string page)
    {
        foreach (var child in FindVisualChildren<RadioButton>(this))
        {
            if (Equals(child.Tag, page))
            {
                child.IsChecked = true;
                break;
            }
        }

        ShowPage(page);
    }

    private void ProfilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedProfile = ProfilesList.SelectedItem as ProfileSummary;
    }

    private void NewProfile_Click(object sender, RoutedEventArgs e)
    {
        var profile = new ProfileSummary($"New profile {Profiles.Count + 1}", false);
        Profiles.Add(profile);
        ProfilesList.SelectedItem = profile;
        StatusMessage = "Created a temporary preview profile. Saving requires Runner IPC.";
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject root) where T : DependencyObject
    {
        for (var index = 0; index < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); index++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(root, index);
            if (child is T typed)
            {
                yield return typed;
            }

            foreach (var nested in FindVisualChildren<T>(child))
            {
                yield return nested;
            }
        }
    }

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
