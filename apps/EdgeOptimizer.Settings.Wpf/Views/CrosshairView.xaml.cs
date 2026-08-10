using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace EdgeOptimizer.Settings.Wpf.Views;

public partial class CrosshairView : UserControl, INotifyPropertyChanged
{
    private int _xOffset;
    private int _yOffset;
    private bool _overlayEnabled = true;
    private string _imageName = "dot-crosshair.png";
    private string _feedbackText = "Preview values are stored in memory only.";

    public CrosshairView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public int XOffset
    {
        get => _xOffset;
        set
        {
            var bounded = Math.Clamp(value, -250, 250);
            if (SetField(ref _xOffset, bounded))
            {
                OnPropertyChanged(nameof(OffsetSummary));
            }
        }
    }

    public int YOffset
    {
        get => _yOffset;
        set
        {
            var bounded = Math.Clamp(value, -250, 250);
            if (SetField(ref _yOffset, bounded))
            {
                OnPropertyChanged(nameof(OffsetSummary));
            }
        }
    }

    public bool OverlayEnabled
    {
        get => _overlayEnabled;
        set => SetField(ref _overlayEnabled, value);
    }

    public string ImageName
    {
        get => _imageName;
        private set => SetField(ref _imageName, value);
    }

    public string FeedbackText
    {
        get => _feedbackText;
        private set => SetField(ref _feedbackText, value);
    }

    public string OffsetSummary => $"Offset X  {XOffset}  •  Y  {YOffset}";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Move_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string direction })
        {
            return;
        }

        switch (direction)
        {
            case "Up": YOffset -= 1; break;
            case "Down": YOffset += 1; break;
            case "Left": XOffset -= 1; break;
            case "Right": XOffset += 1; break;
            default: Center(); break;
        }
    }

    private void Center_Click(object sender, RoutedEventArgs e) => Center();

    private void Center()
    {
        XOffset = 0;
        YOffset = 0;
        FeedbackText = "Crosshair centered in the preview.";
    }

    private void ReplaceImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose a crosshair image",
            Filter = "PNG images (*.png)|*.png",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            ImageName = Path.GetFileName(dialog.FileName);
            FeedbackText = "Image selected for preview. Managed asset storage is not connected yet.";
        }
    }

    private void RemoveImage_Click(object sender, RoutedEventArgs e)
    {
        ImageName = "No image selected";
        FeedbackText = "Crosshair image removed from preview state.";
    }

    private void HidePreview_Click(object sender, RoutedEventArgs e)
    {
        OverlayEnabled = false;
        FeedbackText = "Preview hidden.";
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        OverlayEnabled = true;
        ImageName = "dot-crosshair.png";
        Center();
        FeedbackText = "Crosshair preview reset.";
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
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
