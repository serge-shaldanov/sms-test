using System.Windows;

namespace SmsTest.VariablesApp.Models.Services;

public sealed class WindowsDialogService : IDialogService
{
    public void ShowErrorDialog(string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message);

        MessageBox.Show(
            message,
            caption: "Ошибка",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
}
