using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EdgeOptimizer.Settings.Wpf.Models;

namespace EdgeOptimizer.Settings.Wpf.Views;

public partial class SystemTweaksView : UserControl, INotifyPropertyChanged
{
    private string _processFilter = string.Empty;
    private bool _fanBoostEnabled = true;
    private bool _recycleBinEnabled = true;
    private bool _browserCacheEnabled = true;
    private string _feedbackText = "System tweak values are stored in memory only.";

    public SystemTweaksView()
    {
        Processes.Add(new ProcessItem("Discord.exe", "0.4%", "156.2 MB", true));
        Processes.Add(new ProcessItem("chrome.exe", "1.2%", "512.7 MB", true));
        Processes.Add(new ProcessItem("Spotify.exe", "0.3%", "123.4 MB", true));
        Processes.Add(new ProcessItem("Steam.exe", "0.6%", "287.9 MB", false));

        FilteredProcesses = CollectionViewSource.GetDefaultView(Processes);
        FilteredProcesses.Filter = FilterProcess;
        InitializeComponent();
        DataContext = this;
    }

    public ObservableCollection<ProcessItem> Processes { get; } = new();
    public ICollectionView FilteredProcesses { get; }

    public string ProcessFilter
    {
        get => _processFilter;
        set
        {
            if (SetField(ref _processFilter, value))
            {
                FilteredProcesses.Refresh();
            }
        }
    }

    public bool FanBoostEnabled
    {
        get => _fanBoostEnabled;
        set => SetField(ref _fanBoostEnabled, value);
    }

    public bool RecycleBinEnabled
    {
        get => _recycleBinEnabled;
        set => SetField(ref _recycleBinEnabled, value);
    }

    public bool BrowserCacheEnabled
    {
        get => _browserCacheEnabled;
        set => SetField(ref _browserCacheEnabled, value);
    }

    public string SelectionSummary => $"{Processes.Count(process => process.IsSelected)} selected";

    public string FeedbackText
    {
        get => _feedbackText;
        private set => SetField(ref _feedbackText, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool FilterProcess(object item) => item is ProcessItem process &&
        (string.IsNullOrWhiteSpace(ProcessFilter) || process.Name.Contains(ProcessFilter, StringComparison.OrdinalIgnoreCase));

    private void ProcessSelection_Click(object sender, RoutedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectionSummary));
        FeedbackText = "Updated the temporary process selection.";
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) =>
        FeedbackText = "Process refresh requires Runner IPC; showing preview data.";

    private void ProtectedList_Click(object sender, RoutedEventArgs e) =>
        FeedbackText = "Protected-process details will come from Runner's validated policy.";

    private void RestoreDefaults_Click(object sender, RoutedEventArgs e)
    {
        FanBoostEnabled = false;
        RecycleBinEnabled = false;
        BrowserCacheEnabled = false;
        foreach (var process in Processes)
        {
            process.IsSelected = false;
        }
        OnPropertyChanged(nameof(SelectionSummary));
        FeedbackText = "Preview values restored to safe defaults.";
    }

    private void Save_Click(object sender, RoutedEventArgs e) =>
        FeedbackText = "Save queued in preview only. Runner IPC is required for persistence.";

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
