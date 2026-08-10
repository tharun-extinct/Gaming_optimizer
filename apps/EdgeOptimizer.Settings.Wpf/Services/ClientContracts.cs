using EdgeOptimizer.Settings.Wpf.Models;

namespace EdgeOptimizer.Settings.Wpf.Services;

public interface IFilePicker
{
    string? PickPng();
}

public interface IRunnerClient
{
    bool IsConnected { get; }
    Task SaveProfileAsync(ProfileWorkspace profile, CancellationToken cancellationToken = default);
}

public interface IProcessSource
{
    IReadOnlyList<ProcessItem> GetProcesses();
}

public interface ICleanupClient
{
    Task RequestCleanupAsync(string cleanupKind, CancellationToken cancellationToken = default);
}

public interface IMacroClient
{
    Task TestMacroAsync(MacroDefinition macro, CancellationToken cancellationToken = default);
}

public interface IOverlayClient
{
    Task PreviewAsync(ProfileWorkspace profile, CancellationToken cancellationToken = default);
}
