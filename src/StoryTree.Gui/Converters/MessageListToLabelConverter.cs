using System;
using System.Globalization;
using System.Windows.Data;
using StoryTree.Gui.ViewModels;

namespace StoryTree.Gui.Converters
{
    public class MessageListToLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MessageListViewModel viewModel))
                return value;

            return $"{viewModel.MessageList.Count} Berichten";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}