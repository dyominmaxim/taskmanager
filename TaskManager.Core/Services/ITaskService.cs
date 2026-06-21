using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public interface ITaskService
{
    IReadOnlyList<TaskItem> GetAll();
    IReadOnlyList<TaskItem> GetByStatus(WorkStatus status);
    IReadOnlyList<TaskItem> Search(string query);
    TaskItem Add(TaskItem task);
    void Update(TaskItem task);
    void Delete(int id);
    void SaveToJson(string path);
    void LoadFromJson(string path);
}
