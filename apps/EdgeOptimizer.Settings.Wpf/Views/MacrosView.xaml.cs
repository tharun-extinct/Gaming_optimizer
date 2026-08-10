using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EdgeOptimizer.Settings.Wpf.Models;

namespace EdgeOptimizer.Settings.Wpf.Views;

public partial class MacrosView : UserControl, INotifyPropertyChanged
{
    private MacroDefinition? _selectedMacro;
    private string _macroSearch = string.Empty;
    private string _feedbackText = "Macro edits are stored in memory only.";

    public MacrosView()
    {
        Macros.Add(new MacroDefinition("Build combo", "Ctrl + F8", new[]
        {
            new MacroStep("Key down", "W"),
            new MacroStep("Wait", "120 ms"),
            new MacroStep("Key press", "Space"),
            new MacroStep("Key up", "W")
        }));
        Macros.Add(new MacroDefinition("Quick heal", "Unassigned", new[]
        {
            new MacroStep("Key press", "H")
        }));

        FilteredMacros = CollectionViewSource.GetDefaultView(Macros);
        FilteredMacros.Filter = FilterMacro;
        _selectedMacro = Macros[0];

        InitializeComponent();
        DataContext = this;
    }

    public ObservableCollection<MacroDefinition> Macros { get; } = new();
    public ICollectionView FilteredMacros { get; }

    public MacroDefinition? SelectedMacro
    {
        get => _selectedMacro;
        set => SetField(ref _selectedMacro, value);
    }

    public string MacroSearch
    {
        get => _macroSearch;
        set
        {
            if (SetField(ref _macroSearch, value))
            {
                FilteredMacros.Refresh();
            }
        }
    }

    public string FeedbackText
    {
        get => _feedbackText;
        private set => SetField(ref _feedbackText, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool FilterMacro(object item) => item is MacroDefinition macro &&
        (string.IsNullOrWhiteSpace(MacroSearch) || macro.Name.Contains(MacroSearch, StringComparison.OrdinalIgnoreCase));

    private void MacroList_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        SelectedMacro = (sender as ListBox)?.SelectedItem as MacroDefinition;

    private void NewMacro_Click(object sender, RoutedEventArgs e)
    {
        var macro = new MacroDefinition($"New macro {Macros.Count + 1}", "Unassigned", Array.Empty<MacroStep>());
        Macros.Add(macro);
        SelectedMacro = macro;
        FeedbackText = "Created a temporary macro.";
    }

    private void AddAction_Click(object sender, RoutedEventArgs e)
    {
        SelectedMacro?.Steps.Add(new MacroStep("Key press", "Unassigned"));
        FeedbackText = "Added an action placeholder.";
    }

    private void DeleteStep_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedMacro is not null && sender is Button { Tag: MacroStep step })
        {
            SelectedMacro.Steps.Remove(step);
            FeedbackText = "Removed the action from preview state.";
        }
    }

    private void ChangeShortcut_Click(object sender, RoutedEventArgs e) =>
        FeedbackText = "Shortcut capture will be connected through the macro worker contract.";

    private void DeleteMacro_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedMacro is null)
        {
            return;
        }

        var index = Macros.IndexOf(SelectedMacro);
        Macros.Remove(SelectedMacro);
        SelectedMacro = Macros.Count == 0 ? null : Macros[Math.Clamp(index, 0, Macros.Count - 1)];
        FeedbackText = "Deleted the macro from preview state.";
    }

    private void DuplicateMacro_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedMacro is null)
        {
            return;
        }

        var duplicate = new MacroDefinition($"{SelectedMacro.Name} copy", "Unassigned", SelectedMacro.Steps.ToArray());
        Macros.Add(duplicate);
        SelectedMacro = duplicate;
        FeedbackText = "Duplicated the macro in preview state.";
    }

    private void SaveMacro_Click(object sender, RoutedEventArgs e) =>
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
}
