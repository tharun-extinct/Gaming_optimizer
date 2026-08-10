using System.Collections.ObjectModel;

namespace EdgeOptimizer.Settings.Wpf.Models;

public sealed class MacroDefinition
{
    public MacroDefinition(string name, string shortcut, IEnumerable<MacroStep> steps)
    {
        Name = name;
        Shortcut = shortcut;
        Steps = new ObservableCollection<MacroStep>(steps);
    }

    public string Name { get; set; }
    public string Shortcut { get; set; }
    public ObservableCollection<MacroStep> Steps { get; }
}

public sealed record MacroStep(string Action, string Value);
