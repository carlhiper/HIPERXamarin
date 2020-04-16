using System;
using System.Globalization;
using Xamarin.Forms;

namespace HIPER.ViewModel.Converters
{
    public class ChallengeIdToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string challengeId = (string)value;
            string hiperPeach = "#ff7562";
            string challengeGold = "#D4AF37";

            if (!string.IsNullOrEmpty(challengeId))
            {
                return challengeGold;
            }
  
            return hiperPeach;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
