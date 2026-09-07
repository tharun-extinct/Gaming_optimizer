using EdgeOptimizer.Settings.Core.Models;

namespace EdgeOptimizer.Settings.Core.Services;

public interface IFilePicker
{
    Task<string?> PickPngAsync(CancellationToken cancellationToken = default);
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
