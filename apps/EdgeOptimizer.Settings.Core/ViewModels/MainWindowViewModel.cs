using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EdgeOptimizer.Settings.Core.Models;
using EdgeOptimizer.Settings.Core.Services;

namespace EdgeOptimizer.Settings.Core.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly IRunnerClient _runnerClient;
    private ProfileWorkspace? _selectedProfile;
    private object? _currentPage;
    private string _currentPageLabel = "Dashboard";
    private string _currentPageTitle = "Gaming dashboard";
    private string _statusMessage = "Design preview loaded. Commands remain disabled until Runner IPC is connected.";

    public MainWindowViewModel(IFilePicker filePicker, IRunnerClient runnerClient)
    {
        _runnerClient = runnerClient;
        Dashboard = new DashboardViewModel(NavigateTo);
        Crosshair = new CrosshairViewModel(filePicker);
        Macros = new MacrosViewModel();
        SystemTweaks = new SystemTweaksViewModel();

        NavigateCommand = new RelayCommand<string>(NavigateTo);
        NewProfileCommand = new RelayCommand(NewProfile);

        Profiles.Add(new ProfileWorkspace("Fortnite", true));
        Profiles.Add(new ProfileWorkspace("Valorant", false));
        Profiles.Add(new ProfileWorkspace("Shadow of Tomb Raider", false));
        SelectedProfile = Profiles[0];
        NavigateTo("Dashboard");
    }

    public ObservableCollection<ProfileWorkspace> Profiles { get; } = new();
    public DashboardViewModel Dashboard { get; }
    public CrosshairViewModel Crosshair { get; }
    public MacrosViewModel Macros { get; }
    public SystemTweaksViewModel SystemTweaks { get; }
    public ICommand NavigateCommand { get; }
    public ICommand NewProfileCommand { get; }

    public ProfileWorkspace? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (!SetProperty(ref _selectedProfile, value) || value is null) return;
            Dashboard.LoadProfile(value);
            Crosshair.LoadProfile(value);
            Macros.LoadProfile(value);
            SystemTweaks.LoadProfile(value);
            StatusMessage = $"Loaded {value.Name} preview state. No durable data was changed.";
        }
    }

    public object? CurrentPage { get => _currentPage; private set => SetProperty(ref _currentPage, value); }
    public string CurrentPageLabel { get => _currentPageLabel; private set => SetProperty(ref _currentPageLabel, value); }
    public string CurrentPageTitle { get => _currentPageTitle; private set => SetProperty(ref _currentPageTitle, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public bool ActivationEnabled => _runnerClient.IsConnected;
    public bool IsDashboardSelected => CurrentPageLabel == "Dashboard";
    public bool IsCrosshairSelected => CurrentPageLabel == "Crosshair";
    public bool IsMacrosSelected => CurrentPageLabel == "Macros";
    public bool IsSystemTweaksSelected => CurrentPageLabel == "System Tweaks";

    public void NavigateTo(string? page)
    {
        switch (page)
        {
            case "Crosshair":
                CurrentPage = Crosshair;
                CurrentPageLabel = "Crosshair";
                CurrentPageTitle = "Crosshair overlay";
                break;
            case "Macros":
                CurrentPage = Macros;
                CurrentPageLabel = "Macros";
                CurrentPageTitle = "Macro editor";
                break;
            case "SystemTweaks":
                CurrentPage = SystemTweaks;
                CurrentPageLabel = "System Tweaks";
                CurrentPageTitle = "System tweaks";
                break;
            default:
                CurrentPage = Dashboard;
                CurrentPageLabel = "Dashboard";
                CurrentPageTitle = "Gaming dashboard";
                break;
        }

        OnPropertyChanged(nameof(IsDashboardSelected));
        OnPropertyChanged(nameof(IsCrosshairSelected));
        OnPropertyChanged(nameof(IsMacrosSelected));
        OnPropertyChanged(nameof(IsSystemTweaksSelected));
        StatusMessage = $"Showing {CurrentPageLabel} for {SelectedProfile?.Name ?? "the selected profile"}. Preview data is memory-only.";
    }

    private void NewProfile()
    {
        var profile = new ProfileWorkspace($"New profile {Profiles.Count + 1}", false);
        Profiles.Add(profile);
        SelectedProfile = profile;
        StatusMessage = "Created a temporary preview profile. Saving requires Runner IPC.";
    }
}
