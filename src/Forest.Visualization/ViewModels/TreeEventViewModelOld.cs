﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Forest.Calculators;
using Forest.Data.Estimations;
using Forest.Data.Estimations.PerTreeEvent;
using Forest.Data.Properties;
using Forest.Data.Services;
using Forest.Data.Tree;

namespace Forest.Visualization.ViewModels
{
    public class TreeEventViewModelOld : INotifyPropertyChanged
    {
        private static readonly Dictionary<ProbabilitySpecificationType, string> ProbabilitySpecificationTypes =
            Enum.GetValues(typeof(ProbabilitySpecificationType))
                .Cast<ProbabilitySpecificationType>()
                .ToDictionary(t => t, GetEstimationSpecificationTypeDisplayName);

        private readonly AnalysisManipulationService analysisManipulationService;
        private readonly ObservableCollection<ProbabilityEstimation> probabilityEstimations;
        private TreeEventViewModelOld failingEventViewModel;
        private TreeEventViewModelOld passingEventViewModel;
        private ProbabilitySpecificationViewModelBase probabilityEstimationViewModel;

        public TreeEventViewModelOld([NotNull] TreeEvent treeEvent,
            [NotNull] EventTreeViewModelOld parentEventTreeViewModel,
            [NotNull] AnalysisManipulationService analysisManipulationService,
            [NotNull] ObservableCollection<ProbabilityEstimation> probabilityEstimations)
        {
            this.analysisManipulationService = analysisManipulationService;
            TreeEvent = treeEvent;
            this.probabilityEstimations = probabilityEstimations;
            ParentEventTreeViewModel = parentEventTreeViewModel;
            Estimation = probabilityEstimations?.OfType<ProbabilityEstimationPerTreeEvent>()
                .FirstOrDefault(pe => parentEventTreeViewModel.IsViewModelFor(pe.EventTree));
            if (Estimation != null)
                Estimation.PropertyChanged += EstimationPropertyChanged;

            treeEvent.PropertyChanged += TreeEventPropertyChanged;
        }

        public ProbabilityEstimationPerTreeEvent Estimation { get; }

        public TreeEvent TreeEvent { get; }

        public string Name
        {
            get => TreeEvent.Name;
            set
            {
                TreeEvent.Name = value;
                TreeEvent.OnPropertyChanged();
            }
        }

        public string Summary
        {
            get => TreeEvent.Summary;
            set
            {
                TreeEvent.Summary = value;
                TreeEvent.OnPropertyChanged();
            }
        }

        public TreeEventViewModelOld PassingEvent
        {
            get
            {
                if (TreeEvent?.PassingEvent == null)
                    return null;

                return passingEventViewModel ?? (passingEventViewModel =
                    new TreeEventViewModelOld(TreeEvent.PassingEvent,
                        ParentEventTreeViewModel,
                        analysisManipulationService,
                        probabilityEstimations));
            }
        }

        public TreeEventViewModelOld FailingEvent
        {
            get
            {
                if (TreeEvent?.FailingEvent == null)
                    return null;

                return failingEventViewModel ?? (failingEventViewModel =
                    new TreeEventViewModelOld(TreeEvent.FailingEvent,
                        ParentEventTreeViewModel,
                        analysisManipulationService,
                        probabilityEstimations));
            }
        }

        public bool IsEndPointEvent => TreeEvent.PassingEvent == null && TreeEvent.FailingEvent == null;

        public bool HasTrueEventOnly => TreeEvent.PassingEvent != null && TreeEvent.FailingEvent == null;

        public bool HasFalseEventOnly => TreeEvent.PassingEvent == null && TreeEvent.FailingEvent != null;

        public bool HasTwoEvents => TreeEvent.PassingEvent != null && TreeEvent.FailingEvent != null;

        //public ICommand TreeEventClickedCommand => new TreeEventClickedCommand(this);

        public bool IsSelected => TreeEvent != null && ReferenceEquals(TreeEvent, ParentEventTreeViewModel?.SelectedTreeEvent?.TreeEvent);

        public int ProbabilityEstimationTypeIndex
        {
            get => ProbabilitySpecificationTypes.Keys.ToList().IndexOf(EstimationSpecification.Type);
            set
            {
                var selectedType = ProbabilitySpecificationTypes.ElementAt(value).Key;
                if (EstimationSpecification.Type != selectedType)
                    EstimationSpecification.Estimate.ChangeProbabilityEstimationType(selectedType);
            }
        }

        public IEnumerable<string> EstimationSpecificationOptions => ProbabilitySpecificationTypes.Values;

        public ProbabilitySpecificationViewModelBase EstimationSpecification
        {
            get
            {
                var estimationPerTreeEvent = probabilityEstimations.OfType<ProbabilityEstimationPerTreeEvent>().First();
                return null;
                /*probabilityEstimationViewModel ?? (probabilityEstimationViewModel =
                    ParentEventTreeViewModel.EstimationSpecificationViewModelFactory.CreateViewModel(TreeEvent,
                        estimationPerTreeEvent.Estimates.ToArray(),
                        estimationPerTreeEvent.HydrodynamicConditions));*/
            }
        }

        public TreeEvent[] CriticalPath => TreeEvent == null ? null :
            ParentEventTreeViewModel.MainTreeEventViewModel == null ? null :
            CriticalPathCalculator.GetCriticalPath(ParentEventTreeViewModel.MainTreeEventViewModel.TreeEvent, TreeEvent).ToArray();

        public string Information
        {
            get => TreeEvent.Information;
            set
            {
                TreeEvent.Information = value;
                TreeEvent.OnPropertyChanged();
            }
        }

        public string Discussion { get; set; }

        private EventTreeViewModelOld ParentEventTreeViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void FireSelectedStateChangeRecursive()
        {
            OnPropertyChanged(nameof(IsSelected));
            FailingEvent?.FireSelectedStateChangeRecursive();
            PassingEvent?.FireSelectedStateChangeRecursive();
        }

        public void Select()
        {
            // TODO: Move this to SelectionManager
            ParentEventTreeViewModel.SelectedTreeEvent = this;
        }

        private static string GetEstimationSpecificationTypeDisplayName(ProbabilitySpecificationType t)
        {
            switch (t)
            {
                case ProbabilitySpecificationType.Classes:
                    return "Klassen";
                case ProbabilitySpecificationType.FixedValue:
                    return "Vaste kans";
                case ProbabilitySpecificationType.FixedFrequency:
                    return "Vaste freqeuentielijn";
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void TreeEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TreeEvent.PassingEvent):
                    passingEventViewModel = null;
                    if (PassingEvent == null)
                        // TODO: Shouldn't this be done by the command?
                        Select();
                    OnPropertyChanged(nameof(PassingEvent));
                    OnPropertyChanged(nameof(IsEndPointEvent));
                    OnPropertyChanged(nameof(HasTrueEventOnly));
                    OnPropertyChanged(nameof(HasFalseEventOnly));
                    OnPropertyChanged(nameof(HasTwoEvents));
                    break;
                case nameof(TreeEvent.FailingEvent):
                    failingEventViewModel = null;
                    if (FailingEvent == null)
                        // TODO: Shouldn't this be done by the command?
                        Select();
                    OnPropertyChanged(nameof(FailingEvent));
                    OnPropertyChanged(nameof(IsEndPointEvent));
                    OnPropertyChanged(nameof(HasTrueEventOnly));
                    OnPropertyChanged(nameof(HasFalseEventOnly));
                    OnPropertyChanged(nameof(HasTwoEvents));
                    break;
                case nameof(TreeEvent.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(TreeEvent.Summary):
                    OnPropertyChanged(nameof(Summary));
                    break;
                case nameof(TreeEvent.Information):
                    OnPropertyChanged(nameof(Information));
                    break;
            }
        }

        private void EstimationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TreeEventProbabilityEstimate.ProbabilitySpecificationType):
                    probabilityEstimationViewModel = null;
                    OnPropertyChanged(nameof(ProbabilityEstimationTypeIndex));
                    OnPropertyChanged(nameof(EstimationSpecification));
                    break;
            }
        }
    }
}