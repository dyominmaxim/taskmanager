using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.WPF.Views;

namespace TaskManager.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ITaskService _service;

    // ── Список задач ──────────────────────────────────────────────────────

    private ObservableCollection<TaskItem> _tasks = [];
    public ObservableCollection<TaskItem> Tasks
    {
        get => _tasks;
        private set => Set(ref _tasks, value);
    }

    // ── Выбранная задача ──────────────────────────────────────────────────

    private TaskItem? _selectedTask;
    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set => Set(ref _selectedTask, value);
    }

    // ── Фильтр по статусу ─────────────────────────────────────────────────

    public List<string> StatusFilters { get; } = ["Все", "Новая", "В процессе", "Завершена"];

    private string _selectedStatusFilter = "Все";
    public string SelectedStatusFilter
    {
        get => _selectedStatusFilter;
        set { Set(ref _selectedStatusFilter, value); ApplyFilter(); }
    }

    // ── Поиск ─────────────────────────────────────────────────────────────

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set { Set(ref _searchQuery, value); ApplyFilter(); }
    }

    // ── Статусная строка ──────────────────────────────────────────────────

    private string _statusMessage = "Готово";
    public string StatusMessage
    {
        get => _statusMessage;
        private set => Set(ref _statusMessage, value);
    }

    // ── Команды ───────────────────────────────────────────────────────────

    public ICommand AddCommand    { get; }
    public ICommand EditCommand   { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand   { get; }
    public ICommand LoadCommand   { get; }

    // ── Конструктор ───────────────────────────────────────────────────────

    public MainViewModel() : this(new TaskService()) { }

    public MainViewModel(ITaskService service)
    {
        _service = service;

        AddCommand    = new RelayCommand(AddTask);
        EditCommand   = new RelayCommand(EditTask,   () => SelectedTask is not null);
        DeleteCommand = new RelayCommand(DeleteTask, () => SelectedTask is not null);
        SaveCommand   = new RelayCommand(SaveTasks);
        LoadCommand   = new RelayCommand(LoadTasks);

        ApplyFilter();
    }

    // ── Приватные методы ──────────────────────────────────────────────────

    private void ApplyFilter()
    {
        var all = _service.GetAll().AsEnumerable();

        // Фильтр по статусу
        var statusFilter = GetStatusFilter();
        if (statusFilter.HasValue)
            all = all.Where(t => t.Status == statusFilter.Value);

        // Поиск
        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var q = _searchQuery.ToLower();
            all = all.Where(t =>
                t.Name.ToLower().Contains(q) ||
                t.Description.ToLower().Contains(q));
        }

        Tasks = new ObservableCollection<TaskItem>(all);
        StatusMessage = $"Задач: {Tasks.Count}";
    }

    private TaskStatus? GetStatusFilter() => _selectedStatusFilter switch
    {
        "Новая"       => TaskStatus.New,
        "В процессе"  => TaskStatus.InProgress,
        "Завершена"   => TaskStatus.Completed,
        _              => null
    };

    private void AddTask()
    {
        var dialog = new TaskEditDialog { Owner = Application.Current.MainWindow };
        if (dialog.ShowDialog() != true || dialog.Result is null) return;

        var r    = dialog.Result;
        var task = _service.Add(r.Name, r.Description, r.Priority, r.Deadline, r.Status);
        ApplyFilter();
        SelectedTask  = Tasks.FirstOrDefault(t => t.Id == task.Id);
        StatusMessage = $"Добавлена задача: {task.Name}";
    }

    private void EditTask()
    {
        if (SelectedTask is null) return;
        var dialog = new TaskEditDialog(SelectedTask) { Owner = Application.Current.MainWindow };
        if (dialog.ShowDialog() != true || dialog.Result is null) return;

        _service.Update(dialog.Result);
        var selectedId = SelectedTask.Id;
        ApplyFilter();
        SelectedTask  = Tasks.FirstOrDefault(t => t.Id == selectedId);
        StatusMessage = $"Задача обновлена: {dialog.Result.Name}";
    }

    private void DeleteTask()
    {
        if (SelectedTask is null) return;
        var answer = MessageBox.Show(
            $"Удалить задачу «{SelectedTask.Name}»?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        _service.Delete(SelectedTask.Id);
        ApplyFilter();
        StatusMessage = "Задача удалена";
    }

    private void SaveTasks()
    {
        var dialog = new SaveFileDialog
        {
            Filter      = "JSON файлы (*.json)|*.json",
            DefaultExt  = "json",
            FileName    = "tasks"
        };
        if (dialog.ShowDialog() != true) return;
        _service.SaveToJson(dialog.FileName);
        StatusMessage = "Задачи сохранены";
    }

    private void LoadTasks()
    {
        var dialog = new OpenFileDialog { Filter = "JSON файлы (*.json)|*.json" };
        if (dialog.ShowDialog() != true) return;
        _service.LoadFromJson(dialog.FileName);
        SearchQuery          = "";
        SelectedStatusFilter = "Все";
        ApplyFilter();
        StatusMessage = $"Загружено задач: {Tasks.Count}";
    }
}
