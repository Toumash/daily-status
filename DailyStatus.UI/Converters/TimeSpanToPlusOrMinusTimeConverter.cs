using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DailyStatus.UI.Converters
{
  public class TimeSpanToPlusOrMinusTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      var val = value as TimeSpan?;
      if (!val.HasValue)
        throw new InvalidCastException();

      var _diff = val.Value;
      var sign = "-";
      sign = _diff.TotalHours < 0 ? sign : "";
      return $"{sign}{Math.Abs(_diff.TotalHours):0}:{Math.Abs(_diff.Minutes):00}";

    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
