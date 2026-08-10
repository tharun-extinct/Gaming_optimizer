using Microsoft.Win32;
using EdgeOptimizer.Settings.Wpf.Models;

namespace EdgeOptimizer.Settings.Wpf.Services;

public sealed class WpfFilePicker : IFilePicker
{
    public string? PickPng()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose a crosshair image",
            Filter = "PNG images (*.png)|*.png",
            CheckFileExists = true,
            Multiselect = false
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}

public sealed class DisconnectedRunnerClient : IRunnerClient
{
    public bool IsConnected => false;

    public Task SaveProfileAsync(ProfileWorkspace profile, CancellationToken cancellationToken = default) =>
        Task.FromException(new InvalidOperationException("Runner IPC is not connected."));
}
