using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RustMapsApi.Assets.GalleryApp;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
