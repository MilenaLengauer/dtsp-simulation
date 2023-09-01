using System;
using System.Globalization;
using System.Windows.Data;

namespace TSPSimulation.UserInterface.Util
{
    public class TimeCityTupleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tuple = value as (decimal Time, int City)?;

            if (tuple == null)
                return null;

            return new TimeCityTuple(tuple.Value.Time, tuple.Value.City);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var record = value as TimeCityTuple;

            if (record == null)
                return null;

            return (record.Time, record.City);
        }
    }

    public record TimeCityTuple (decimal Time, int City) { }

}
