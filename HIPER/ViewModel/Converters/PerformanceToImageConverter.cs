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
                return "veryhappy.png";
            }else if (indicator == 4)
            {
                return "happy.png";
            }
            else if (indicator == 3)
            {
                return "pleased.png";
            }
            else if (indicator == 2)
            {
                return "sad.png";
            }
            else if (indicator == 1)
            {
                return "verysad.png";
            }
            else { return null; }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
