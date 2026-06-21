using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TaskManager.App.Views;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.App.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly TaskService _service = new();

    private ObservableCollection<TaskViewModel> _tasks = [];
    public ObservableCollection<TaskViewModel> Tasks
    {
        get => _tasks;
        private set => Set(ref _tasks, value);
    }

    private TaskViewModel? _selectedTask;
    public TaskViewModel? SelectedTask
    {
        get => _selectedTask;
        set => Set(ref _selectedTask, value);
    }

    private string _filterStatus = "Все";
    public string FilterStatus
    {
        get => _filterStatus;
        set { Set(ref _filterStatus, value); Refresh(); }
    }

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set { Set(ref _searchQuery, value); Refresh(); }
    }

    private string _statusMessage = "Готово";
    public string StatusMessage
    {
        get => _statusMessage;
        private set => Set(ref _statusMessage, value);
    }

    public List<string> StatusFilters { get; } = ["Все", "Новая", "В процессе", "Завершена"];

    public ICommand NewTaskCommand    { get; }
    public ICommand EditTaskCommand   { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand SaveCommand       { get; }
    public ICommand LoadCommand       { get; }

    public MainViewModel()
    {
        NewTaskCommand    = new RelayCommand(NewTask);
        EditTaskCommand   = new RelayCommand(EditTask,   () => SelectedTask != null);
        DeleteTaskCommand = new RelayCommand(DeleteTask, () => SelectedTask != null);
        SaveCommand       = new RelayCommand(Save);
        LoadCommand       = new RelayCommand(Load);
        Refresh();
    }

    private void Refresh()
    {
        WorkStatus? status = _filterStatus switch
        {
            "Новая"      => WorkStatus.New,
            "В процессе" => WorkStatus.InProgress,
            "Завершена"  => WorkStatus.Completed,
            _            => null
        };

        var tasks = _service.GetAll().AsEnumerable();

        if (status.HasValue)
            tasks = tasks.Where(t => t.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var q = _searchQuery.ToLower();
            tasks = tasks.Where(t =>
                t.Title.ToLower().Contains(q) ||
                t.Description.ToLower().Contains(q));
        }

        Tasks = new ObservableCollection<TaskViewModel>(tasks.Select(t => new TaskViewModel(t)));
        StatusMessage = $"Задач: {Tasks.Count}";
    }

    private void NewTask()
    {
        var task = new TaskItem();
        var dialog = new TaskEditDialog(new TaskViewModel(task))
        {
            Owner = Application.Current.MainWindow
        };
        if (dialog.ShowDialog() != true) return;
        _service.Add(task);
        Refresh();
        StatusMessage = "Задача создана";
    }

    private void EditTask()
    {
        if (SelectedTask is null) return;

        var copy = new TaskItem
        {
            Id          = SelectedTask.Id,
            Title       = SelectedTask.Title,
            Description = SelectedTask.Description,
            Priority    = SelectedTask.Priority,
            DueDate     = SelectedTask.DueDate,
            Status      = SelectedTask.Status,
            CreatedAt   = SelectedTask.CreatedAt
        };

        var dialog = new TaskEditDialog(new TaskViewModel(copy))
        {
            Owner = Application.Current.MainWindow
        };
        if (dialog.ShowDialog() != true) return;
        _service.Update(copy);
        Refresh();
        StatusMessage = "Задача обновлена";
    }

    private void DeleteTask()
    {
        if (SelectedTask is null) return;

        var answer = MessageBox.Show(
            $"Удалить «{SelectedTask.Title}»?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer != MessageBoxResult.Yes) return;

        _service.Delete(SelectedTask.Id);
        Refresh();
        StatusMessage = "Задача удалена";
    }

    private void Save()
    {
        var dialog = new SaveFileDialog
        {
            FileName = "tasks.json",
            Filter   = "JSON (*.json)|*.json|Все файлы|*.*"
        };
        if (dialog.ShowDialog() != true) return;
        _service.SaveToJson(dialog.FileName);
        StatusMessage = $"Сохранено: {dialog.FileName}";
    }

    private void Load()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|Все файлы|*.*"
        };
        if (dialog.ShowDialog() != true) return;
        try
        {
            _service.LoadFromJson(dialog.FileName);
            Refresh();
            StatusMessage = $"Загружено: {dialog.FileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
