﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using StoryTree.Data;
using StoryTree.Data.Properties;

namespace StoryTree.Gui.ViewModels
{
    public class FragilityCurveViewModel : INotifyPropertyChanged
    {
        public FragilityCurveViewModel(FragilityCurve curve)
        {
            FragilityCurve = curve;
        }

        public FragilityCurve FragilityCurve { get; }

        public ObservableCollection<FragilityCurveElementViewModel> CurveElements =>
            new ObservableCollection<FragilityCurveElementViewModel>(FragilityCurve.Select(ce => new FragilityCurveElementViewModel(ce)));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}