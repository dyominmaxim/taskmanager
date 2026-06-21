namespace TaskManager.Core.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? DueDate { get; set; }
    public WorkStatus Status { get; set; } = WorkStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
