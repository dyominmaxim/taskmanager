using System.Globalization;
using System.Windows.Data;
using TaskManager.Core.Models;

namespace TaskManager.App.Converters;

public class EnumDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            Priority.Low          => "Низкий",
            Priority.Medium       => "Средний",
            Priority.High         => "Высокий",
            WorkStatus.New        => "Новая",
            WorkStatus.InProgress => "В процессе",
            WorkStatus.Completed  => "Завершена",
            _                     => value?.ToString() ?? ""
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
