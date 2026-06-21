using TaskManager.Core.Models;

namespace TaskManager.App.ViewModels;

public class TaskViewModel(TaskItem task) : BaseViewModel
{
    public int Id => task.Id;

    public string Title
    {
        get => task.Title;
        set { task.Title = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => task.Description;
        set { task.Description = value; OnPropertyChanged(); }
    }

    public Priority Priority
    {
        get => task.Priority;
        set { task.Priority = value; OnPropertyChanged(); }
    }

    public DateTime? DueDate
    {
        get => task.DueDate;
        set { task.DueDate = value; OnPropertyChanged(); }
    }

    public WorkStatus Status
    {
        get => task.Status;
        set { task.Status = value; OnPropertyChanged(); }
    }

    public DateTime CreatedAt => task.CreatedAt;

    public TaskItem Task => task;
}
