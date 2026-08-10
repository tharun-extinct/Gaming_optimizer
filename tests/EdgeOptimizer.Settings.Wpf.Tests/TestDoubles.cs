using EdgeOptimizer.Settings.Wpf.Models;
using EdgeOptimizer.Settings.Wpf.Services;

namespace EdgeOptimizer.Settings.Wpf.Tests;

internal sealed class FakeFilePicker(string? path) : IFilePicker
{
    public string? PickPng() => path;
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
