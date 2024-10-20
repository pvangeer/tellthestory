﻿using System;
using System.Windows.Input;

namespace StoryTree.Gui.Command
{
    public class OpenProjectCommand : ICommand
    {
        private readonly GuiProjectServices guiProjectServices;

        public OpenProjectCommand(GuiProjectServices guiProjectServices)
        {
            this.guiProjectServices = guiProjectServices;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            guiProjectServices.OpenProject();
        }

        public event EventHandler CanExecuteChanged;
    }
}