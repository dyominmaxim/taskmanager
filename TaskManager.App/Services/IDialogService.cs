using TaskManager.App.ViewModels;

namespace TaskManager.App.Services;

public interface IDialogService
{
    bool ShowTaskDialog(TaskViewModel vm);
    bool ConfirmDelete(string title);
    string? ShowSaveDialog(string defaultFileName);
    string? ShowOpenDialog();
}
