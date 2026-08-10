using System.ComponentModel;
using System.Windows.Input;
using EdgeOptimizer.Settings.Wpf.Infrastructure;
using EdgeOptimizer.Settings.Wpf.Models;

namespace EdgeOptimizer.Settings.Wpf.ViewModels;

public sealed class SystemTweaksViewModel : ObservableObject
{
    private ProfileWorkspace? _profile;
    private string _processFilter = string.Empty;
    private string _feedbackText = "System tweak values are stored in memory only.";

    public SystemTweaksViewModel()
    {
        RefreshCommand = new RelayCommand(() => FeedbackText = "Process refresh requires Runner IPC; showing preview data.");
        ProtectedListCommand = new RelayCommand(() => FeedbackText = "Protected-process details will come from Runner's validated policy.");
        RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
        SaveCommand = new RelayCommand(() => FeedbackText = "Save queued in preview only. Runner IPC is required for persistence.");
    }

    public ICommand RefreshCommand { get; }
    public ICommand ProtectedListCommand { get; }
    public ICommand RestoreDefaultsCommand { get; }
    public ICommand SaveCommand { get; }

    public IEnumerable<ProcessItem> FilteredProcesses =>
        (_profile?.Processes.AsEnumerable() ?? Enumerable.Empty<ProcessItem>()).Where(FilterProcess);
    public string SelectionSummary => $"{_profile?.Processes.Count(process => process.IsSelected) ?? 0} selected";

    public string ProcessFilter
    {
        get => _processFilter;
        set
        {
            if (SetProperty(ref _processFilter, value)) OnPropertyChanged(nameof(FilteredProcesses));
        }
    }

    public bool FanBoostEnabled
    {
        get => _profile?.FanBoostEnabled ?? false;
        set { if (_profile is not null && _profile.FanBoostEnabled != value) { _profile.FanBoostEnabled = value; OnPropertyChanged(); } }
    }

    public bool RecycleBinEnabled
    {
        get => _profile?.RecycleBinEnabled ?? false;
        set { if (_profile is not null && _profile.RecycleBinEnabled != value) { _profile.RecycleBinEnabled = value; OnPropertyChanged(); } }
    }

    public bool BrowserCacheEnabled
    {
        get => _profile?.BrowserCacheEnabled ?? false;
        set { if (_profile is not null && _profile.BrowserCacheEnabled != value) { _profile.BrowserCacheEnabled = value; OnPropertyChanged(); } }
    }

    public string FeedbackText { get => _feedbackText; private set => SetProperty(ref _feedbackText, value); }

    public void LoadProfile(ProfileWorkspace profile)
    {
        if (_profile is not null)
        {
            foreach (var process in _profile.Processes) process.PropertyChanged -= ProcessPropertyChanged;
        }
        _profile = profile;
        foreach (var process in profile.Processes) process.PropertyChanged += ProcessPropertyChanged;
        OnPropertyChanged(nameof(FilteredProcesses));
        OnPropertyChanged(nameof(SelectionSummary));
        OnPropertyChanged(nameof(FanBoostEnabled));
        OnPropertyChanged(nameof(RecycleBinEnabled));
        OnPropertyChanged(nameof(BrowserCacheEnabled));
    }

    private bool FilterProcess(ProcessItem process) =>
        string.IsNullOrWhiteSpace(ProcessFilter) || process.Name.Contains(ProcessFilter, StringComparison.OrdinalIgnoreCase);

    private void ProcessPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProcessItem.IsSelected)) OnPropertyChanged(nameof(SelectionSummary));
    }

    private void RestoreDefaults()
    {
        if (_profile is null) return;
        FanBoostEnabled = false;
        RecycleBinEnabled = false;
        BrowserCacheEnabled = false;
        foreach (var process in _profile.Processes) process.IsSelected = false;
        FeedbackText = "Preview values restored to safe defaults.";
    }
}
