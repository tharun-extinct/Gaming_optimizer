using EdgeOptimizer.Settings.Core.Models;
using EdgeOptimizer.Settings.Core.Services;
using Windows.Storage.Pickers;

namespace EdgeOptimizer.Settings.WinUI.Services;

public sealed class WinUIFilePicker : IFilePicker
{
    public async Task<string?> PickPngAsync(CancellationToken cancellationToken = default)
    {
        var window = App.Window ?? throw new InvalidOperationException("The main window is not available.");
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary, ViewMode = PickerViewMode.Thumbnail };
        picker.FileTypeFilter.Add(".png");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(window));
        var file = await picker.PickSingleFileAsync().AsTask(cancellationToken);
        return file?.Path;
    }
}

public sealed class DisconnectedRunnerClient : IRunnerClient
{
    public bool IsConnected => false;
    public Task SaveProfileAsync(ProfileWorkspace profile, CancellationToken cancellationToken = default) => Task.FromException(new InvalidOperationException("Runner IPC is not connected."));
}
