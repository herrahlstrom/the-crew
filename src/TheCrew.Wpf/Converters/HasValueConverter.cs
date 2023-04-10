using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace TheCrew.Wpf.Converters;

public class HasValueConverter : IValueConverter
{
   public bool ReturnVisibility { get; set; }

   public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
   {
      if (ReturnVisibility)
      {
         return HasValue(value)
            ? Visibility.Visible
            : Visibility.Collapsed;
      }

      return HasValue(value);
   }

   public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
   {
      throw new NotImplementedException();
   }

   private static bool HasValue(object? value)
   {
      if (value is string str)
      {
         return !string.IsNullOrWhiteSpace(str);
      }

      return value is not null;
   }
}