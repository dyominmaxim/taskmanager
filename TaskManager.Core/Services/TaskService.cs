using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class TaskService : ITaskService
{
    private List<TaskItem> _tasks = [];
    private int _next = 1;

    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public IReadOnlyList<TaskItem> GetAll() => _tasks.AsReadOnly();

    public IReadOnlyList<TaskItem> GetByStatus(WorkStatus s) =>
        _tasks.Where(t => t.Status == s).ToList();

    public IReadOnlyList<TaskItem> Search(string q) =>
        string.IsNullOrWhiteSpace(q)
            ? GetAll()
            : _tasks.Where(t =>
                t.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

    public TaskItem Add(TaskItem t)
    {
        t.Id = _next++;
        t.CreatedAt = DateTime.Now;
        _tasks.Add(t);
        return t;
    }

    public void Update(TaskItem t)
    {
        var i = _tasks.FindIndex(x => x.Id == t.Id);
        if (i == -1) throw new KeyNotFoundException($"Id={t.Id}");
        _tasks[i] = t;
    }

    public void Delete(int id)
    {
        var t = _tasks.FirstOrDefault(x => x.Id == id);
        if (t != null) _tasks.Remove(t);
    }

    public void SaveToJson(string path) =>
        File.WriteAllText(path, JsonSerializer.Serialize(_tasks, Json));

    public void LoadFromJson(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Файл не найден.", path);
        _tasks = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(path), Json) ?? [];
        _next = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
    }
}
