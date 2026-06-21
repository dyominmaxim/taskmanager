using System.Windows;
using TaskManager.Core.Models;

namespace TaskManager.WPF.Views;

public partial class TaskEditDialog : Window
{
    private readonly TaskItem? _original;
    public TaskItem? Result { get; private set; }

    public TaskEditDialog(TaskItem? task = null)
    {
        InitializeComponent();
        _original = task;
        if (task != null) PopulateForm(task);
    }

    private void PopulateForm(TaskItem task)
    {
        NameBox.Text              = task.Name;
        DescBox.Text              = task.Description;
        PriorityCombo.SelectedIndex = (int)task.Priority;
        DeadlinePicker.SelectedDate = task.Deadline;
        StatusCombo.SelectedIndex   = (int)task.Status;
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Введите название задачи.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new TaskItem
        {
            Id          = _original?.Id ?? 0,
            Name        = NameBox.Text.Trim(),
            Description = DescBox.Text.Trim(),
            Priority    = (TaskPriority)PriorityCombo.SelectedIndex,
            Deadline    = DeadlinePicker.SelectedDate,
            Status      = (TaskStatus)StatusCombo.SelectedIndex,
            CreatedAt   = _original?.CreatedAt ?? DateTime.Now
        };
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}
