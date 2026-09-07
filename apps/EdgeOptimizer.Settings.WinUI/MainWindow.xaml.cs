using WinUIEx;

namespace EdgeOptimizer.Settings.WinUI;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow(ShellPage shell)
    {
        InitializeComponent();
        Root.Children.Add(shell);
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(shell.DragRegion);
        CenterOnScreen();
    }
}
