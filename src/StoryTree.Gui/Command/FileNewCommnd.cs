﻿using System;
using System.Windows.Input;
using StoryTree.Data;
using StoryTree.Gui.ViewModels;

namespace StoryTree.Gui.Command
{
    public class FileNewCommnd : ICommand
    {
        public FileNewCommnd(GuiViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public GuiViewModel ViewModel { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.GuiProjectSercices.NewProject();
        }

        public event EventHandler CanExecuteChanged;
    }
}