using System;
using System.Globalization;
using Xamarin.Forms;

namespace HIPER.ViewModel.Converters
{
    public class AcceptedChallengeToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool accepted = (bool)value;
            string hiperPeach = "#ff7562";
            string hiperGray = "#777777";

            if (accepted)
            {
                return hiperPeach;
            }
  
            return hiperGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
