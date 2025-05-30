using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace NotaDog.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string? name = value.ToString();
                if (name != null)
                {
                    FieldInfo? fi = value.GetType().GetField(name);
                    if (fi != null)
                    {
                        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attributes.Length > 0)
                            return attributes[0].Description;
                        else
                            return value.ToString();
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implémentation non nécessaire pour cet usage
            throw new NotImplementedException();
        }
    }
}
