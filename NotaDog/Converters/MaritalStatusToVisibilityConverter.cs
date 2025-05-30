using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NotaDog.Data;

namespace NotaDog.Converters
{
    public class MaritalStatusToVisibilityConverter : IValueConverter
    {
        // Propriété pour inverser la logique si nécessaire
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MaritalStatus status)
            {
                bool isVisible = status != MaritalStatus.None;

                // Inverser la logique si Invert est true
                if (Invert)
                {
                    isVisible = !isVisible;
                }

                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            // Si la valeur n'est pas un MaritalStatus valide, on cache le contrôle
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}