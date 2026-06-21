using System.Windows;
using TaskManager.App.ViewModels;
using TaskManager.Core.Models;

namespace TaskManager.App.Views;

public partial class TaskEditDialog : Window
{
    public static IEnumerable<Priority>   PriorityValues => Enum.GetValues<Priority>();
    public static IEnumerable<WorkStatus> StatusValues   => Enum.GetValues<WorkStatus>();

    public TaskEditDialog(TaskViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(((TaskViewModel)DataContext).Title))
        {
            MessageBox.Show("Введите название задачи.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TitleBox.Focus();
            return;
        }
        DialogResult = true;
    }
}
