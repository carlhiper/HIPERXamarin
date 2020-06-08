using System;
using System.Globalization;
using Xamarin.Forms;

namespace HIPER.ViewModel.Converters
{
    public class DateTimeToTimeLeft : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset dateTime = (DateTimeOffset)value;
            DateTimeOffset rightNow = DateTimeOffset.Now;
            var differerence = rightNow - dateTime;

            if (differerence.TotalDays < 1)
                return "today";
            else
            {
                return $"in {differerence.TotalDays:0} days";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
