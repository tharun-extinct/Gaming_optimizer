using EdgeOptimizer.Settings.Core.Models;
using EdgeOptimizer.Settings.Core.Services;

namespace EdgeOptimizer.Settings.Core.Tests;

internal sealed class FakeFilePicker(string? path) : IFilePicker
{
    public Task<string?> PickPngAsync(CancellationToken cancellationToken = default) => Task.FromResult(path);
}

internal sealed class FakeRunnerClient(bool connected = false) : IRunnerClient
{
    public bool IsConnected => connected;
    public List<ProfileWorkspace> SavedProfiles { get; } = new();

    public Task SaveProfileAsync(ProfileWorkspace profile, CancellationToken cancellationToken = default)
    {
        SavedProfiles.Add(profile);
        return Task.CompletedTask;
    }
}
