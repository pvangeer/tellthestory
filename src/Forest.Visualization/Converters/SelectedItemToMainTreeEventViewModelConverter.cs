using System;
using System.Globalization;
using System.Windows.Data;
using Forest.Visualization.ViewModels;

namespace Forest.Visualization.Converters
{
    public class SelectedItemToMainTreeEventViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EventTreeViewModelOld eventTreeViewModel))
                return null;

            return eventTreeViewModel.MainTreeEventViewModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}