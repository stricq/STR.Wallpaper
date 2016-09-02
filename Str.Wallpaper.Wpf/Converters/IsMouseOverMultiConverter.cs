using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;


namespace Str.Wallpaper.Wpf.Converters {

  internal sealed class IsMouseOverMultiConverter : IMultiValueConverter {

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
      return values.Aggregate(false, (seed, value) => seed | (bool)value);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

  }

}
