using Microsoft.Win32;
using System.Windows;
using TaskManager.App.ViewModels;
using TaskManager.App.Views;

namespace TaskManager.App.Services;

public class DialogService : IDialogService
{
    public bool ShowTaskDialog(TaskViewModel vm)
        => new TaskEditDialog(vm) { Owner = Application.Current.MainWindow }.ShowDialog() == true;

    public bool ConfirmDelete(string title)
        => MessageBox.Show($"Удалить «{title}»?", "Подтверждение",
               MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public string? ShowSaveDialog(string name)
    {
        var d = new SaveFileDialog { FileName = name, Filter = "JSON (*.json)|*.json|Все файлы|*.*" };
        return d.ShowDialog() == true ? d.FileName : null;
    }

    public string? ShowOpenDialog()
    {
        var d = new OpenFileDialog { Filter = "JSON (*.json)|*.json|Все файлы|*.*" };
        return d.ShowDialog() == true ? d.FileName : null;
    }
}
