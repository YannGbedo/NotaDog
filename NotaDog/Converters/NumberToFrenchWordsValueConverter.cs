using System;
using System.Globalization;
using System.Windows.Data;
using NotaDog.Services;

namespace NotaDog.Converters
{
    public class NumberToFrenchWordsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;

            if (double.TryParse(value.ToString(), NumberStyles.Number, culture, out double number))
            {
                return NumberToFrenchWordsConverter.ConvertToFrenchWords(number);
            }

            return "Valeur invalide";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Non implémenté
            throw new NotImplementedException();
        }
    }
}
