using System.Globalization;
using System.Windows.Data;
using TaskManager.Core.Models;

namespace TaskManager.WPF.Converters;

public class PriorityToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is TaskPriority p ? p switch
        {
            TaskPriority.Low    => "Низкий",
            TaskPriority.Medium => "Средний",
            TaskPriority.High   => "Высокий",
            _                   => ""
        } : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class StatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is TaskStatus s ? s switch
        {
            TaskStatus.New        => "Новая",
            TaskStatus.InProgress => "В процессе",
            TaskStatus.Completed  => "Завершена",
            _                     => ""
        } : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
