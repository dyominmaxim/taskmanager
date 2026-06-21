using System.Windows;
using System.Windows.Input;
using TaskManager.App.ViewModels;

namespace TaskManager.App.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedTask != null)
            vm.EditTaskCommand.Execute(null);
    }
}
