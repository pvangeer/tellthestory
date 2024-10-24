using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forest.Visualization.Commands
{
    public class SelectTodayCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ((Calendar)parameter).SelectedDate = DateTime.Now.Date;
        }

        public event EventHandler CanExecuteChanged;
    }
}