﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Forest.Data;
using Forest.Gui.Components;
using Forest.Gui.ViewModels;

namespace Forest.Gui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                e.Cancel = !viewModel.ForcedClosingMainWindow();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel(new ForestGui
            {
                ForestAnalysis = ForestAnalysisFactory.CreateStandardNewProject()
            });
        }
    }
}