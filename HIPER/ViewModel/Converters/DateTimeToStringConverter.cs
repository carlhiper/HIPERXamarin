using System;
using System.Globalization;
using Xamarin.Forms;

namespace HIPER.ViewModel.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            DateTimeOffset dateTime = (DateTimeOffset)value;
            DateTimeOffset rightNow = DateTimeOffset.Now;
            var differerence = rightNow - dateTime;

            if (differerence.TotalDays >= 7)
                return $"{dateTime:d}";
            else
            {
                if (differerence.TotalSeconds < 60)
                    return $"{differerence.TotalSeconds:0} seconds ago";
                if (differerence.TotalMinutes < 60)
                    return $"{differerence.TotalMinutes:0} minutes ago";
                if (differerence.TotalHours < 24)
                    return $"{differerence.TotalHours:0} hours ago";
                if (differerence.TotalDays >= 1 && differerence.TotalDays < 2)
                    return "yesterday";


                return $"{differerence.TotalDays:0} days ago";

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTimeOffset.Now;
        }
    }
}
