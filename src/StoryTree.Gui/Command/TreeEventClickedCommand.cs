﻿using System;
using System.Windows.Input;
using StoryTree.Gui.ViewModels;

namespace StoryTree.Gui.Command
{
    public class TreeEventClickedCommand : ICommand
    {
        public TreeEventClickedCommand(TreeEventViewModel treeEventViewModel)
        {
            TreeEventViewModel = treeEventViewModel;
        }

        public TreeEventViewModel TreeEventViewModel { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TreeEventViewModel.Select();
        }

        public event EventHandler CanExecuteChanged;
    }
}