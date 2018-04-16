﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StoryTree.Data;
using StoryTree.Data.Annotations;
using StoryTree.Gui.Messaging;
using StoryTree.Gui.ViewModels;

namespace StoryTree.Gui
{
    public class StoryTreeGui : INotifyPropertyChanged
    {
        public StoryTreeGui()
        {
            BusyIndicator = StorageState.Idle;
            Messages = new ObservableCollection<StoryTreeMessage>();
            Project = new Project();
            ProjectFilePath = "";
        }

        public StorageState BusyIndicator { get; set; }

        public Project Project { get; set; }

        public string ProjectFilePath { get; set; }

        public ObservableCollection<StoryTreeMessage> Messages { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}