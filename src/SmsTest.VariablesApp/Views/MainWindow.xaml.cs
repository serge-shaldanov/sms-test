using System.Windows;
using System.Windows.Input;

using SmsTest.VariablesApp.ViewModels;

namespace SmsTest.VariablesApp.Views;

public partial class MainWindow
{
    public MainWindow(MainViewModel mainViewModel)
    {
        ArgumentNullException.ThrowIfNull(mainViewModel);

        this.InitializeComponent();

        this.DataContext = mainViewModel;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
