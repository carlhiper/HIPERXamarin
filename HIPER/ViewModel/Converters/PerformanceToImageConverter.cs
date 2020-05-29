using System;
using System.Globalization;
using Xamarin.Forms;

namespace HIPER.ViewModel.Converters
{
    public class PerformanceToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int indicator = (int)value;
            if(indicator == 5)
            {
                return "arrow_up.png";
            }else if (indicator == 4)
            {
                return "arrow_neutral_up.png";
            }
            else if (indicator == 3)
            {
                return "arrow_neutral.png";
            }
            else if (indicator == 2)
            {
                return "arrow_neutral_down.png";
            }
            else if (indicator == 1)
            {
                return "arrow_down.png";
            }
            else { return null; }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
