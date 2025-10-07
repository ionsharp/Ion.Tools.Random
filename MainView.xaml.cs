using System.Windows;

namespace Ion.Tools.Random;

public partial class MainWindow : Window
{
    private MainViewModel viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void onCopy(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Clipboard.SetText(viewModel.Text);
    }

    private void onDo(object sender, RoutedEventArgs e)
    {
        viewModel.Do();
    }
}