using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using StoryTree.Data;
using StoryTree.Data.Properties;
using StoryTree.Data.Services;
using StoryTree.Gui.Command;

namespace StoryTree.Gui.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private readonly AddTreeEventCommand addTreeEventCommand;
        private readonly RemoveTreeEventCommand removeTreeEventCommand;

        public ProjectViewModel() : this(new Project()) { }

        public ProjectViewModel([NotNull]Project project)
        {
            BusyIndicator = StorageState.Idle;

            Project = project;
            projectManipulationService = new ProjectManipulationService(project);

            var eventTreeViewModels = new ObservableCollection<EventTreeViewModel>(project.EventTrees.Select(te =>
            {
                var eventTreeViewModel = new EventTreeViewModel(te, projectManipulationService)
                {
                    EstimationSpecificationViewModelFactory = new EstimationSpecificationViewModelFactory(project)
                };
                eventTreeViewModel.PropertyChanged += EventTreeViewModelPropertyChanged;
                return eventTreeViewModel;
            }));

            EventTrees = eventTreeViewModels;
            addTreeEventCommand = new AddTreeEventCommand(this);
            removeTreeEventCommand = new RemoveTreeEventCommand(this);

            expertViewModels = new ObservableCollection<ExpertViewModel>(Project.Experts.Select(e => new ExpertViewModel(e)));
            expertViewModels.CollectionChanged += ExpertViewModelsCollectionChanged;

            hydraulicsViewModels = new ObservableCollection<HydraulicConditionViewModel>(Project.HydraulicConditions.Select(e => new HydraulicConditionViewModel(e)));
            hydraulicsViewModels.CollectionChanged += HydraulicsViewModelsCollectionChanged;

            project.EventTrees.CollectionChanged += EventTreesCollectionChanged;

            SelectedEventTreeFiltered = EventTrees.FirstOrDefault();
            foreach (var eventTreeViewModel in EventTrees)
            {
                eventTreeViewModel.SelectedTreeEvent = eventTreeViewModel.MainTreeEventViewModel;
            }
        }

        public Project Project { get; }

        public IEnumerable<EventTreeViewModel> EventTreesToEstimate => EventTrees.Where(e => e.NeedsSpecification);

        public ObservableCollection<EventTreeViewModel> EventTrees { get; }

        public string ProjectName
        {
            get => Project.Name;
            set => Project.Name = value;
        }

        public string ProjectDescription
        {
            get => Project.Description;
            set => Project.Description = value;
        }

        public string AssessmentSection
        {
            get => Project.AssessmentSection;
            set => Project.AssessmentSection = value;
        }

        public string ProjectInformation
        {
            get => Project.ProjectInformation;
            set => Project.ProjectInformation = value;
        }

        public ICommand AddEventTreeCommand => new AddEventTreeCommand(this);

        public ICommand RemoveEventTreeCommand => new RemoveEventTreeCommand(this);

        public ICommand RemoveTreeEventCommand => removeTreeEventCommand;

        public ICommand AddTreeEventCommand => addTreeEventCommand;

        private EventTreeViewModel selectedEventTree;
        private readonly ObservableCollection<ExpertViewModel> expertViewModels;
        private readonly ObservableCollection<HydraulicConditionViewModel> hydraulicsViewModels;
        private ProjectManipulationService projectManipulationService;

        public EventTreeViewModel SelectedEventTreeUnFiltered
        {
            get => selectedEventTree;
            set => SelectedEventTreeFiltered = value;
        }

        public EventTreeViewModel SelectedEventTreeFiltered
        {
            get => selectedEventTree == null ? null :
                selectedEventTree.NeedsSpecification ? selectedEventTree : null;
            set
            {
                selectedEventTree = value;
                OnPropertyChanged(nameof(SelectedEventTreeFiltered));
                OnPropertyChanged(nameof(SelectedEventTreeUnFiltered));
                OnPropertyChanged(nameof(SelectedTreeEvent));
                foreach (var eventTreeViewModel in EventTrees)
                {
                    eventTreeViewModel.IsSelected = Equals(selectedEventTree, eventTreeViewModel);
                }
                addTreeEventCommand.FireCanExecuteChanged();
                removeTreeEventCommand.FireCanExecuteChanged();
            }
        }

        public TreeEventViewModel SelectedTreeEvent => SelectedEventTreeFiltered?.SelectedTreeEvent;

        public ObservableCollection<ExpertViewModel> Experts => expertViewModels;

        public string ProjectLeaderName
        {
            get => Project.ProjectLeader.Name;
            set
            {
                Project.ProjectLeader.Name = value;
                Project.ProjectLeader.OnPropertyChanged(nameof(Project.ProjectLeader.Name));
            }
        }

        public string ProjectLeaderEmail
        {
            get => Project.ProjectLeader.Email;
            set
            {
                Project.ProjectLeader.Email = value;
                Project.ProjectLeader.OnPropertyChanged(nameof(Project.ProjectLeader.Email));
            }
        }

        public string ProjectLeaderTelephone
        {
            get => Project.ProjectLeader.Telephone;
            set
            {
                Project.ProjectLeader.Telephone = value;
                Project.ProjectLeader.OnPropertyChanged(nameof(Project.ProjectLeader.Telephone));
            }
        }

        public ObservableCollection<HydraulicConditionViewModel> HydraulicConditionsList => hydraulicsViewModels;

        public StorageState BusyIndicator { get; set; }

        public DateTime StartDate
        {
            get => Project.StartDate;
            set
            {
                Project.StartDate = value;
                Project.OnPropertyChanged(nameof(Project.StartDate));
            }
        }

        public DateTime EndDate
        {
            get => Project.EndDate;
            set
            {
                Project.EndDate = value;
                Project.OnPropertyChanged(nameof(Project.EndDate));
            }
        }

        public void AddNewEventTree()
        {
            var eventTree = new EventTree {Name = "Nieuwe gebeurtenis"};
            Project.EventTrees.Add(eventTree);
            OnPropertyChanged(nameof(EventTrees));
            OnPropertyChanged(nameof(EventTreesToEstimate));
            SelectedEventTreeFiltered = EventTrees.FirstOrDefault(et => et.IsViewModelFor(eventTree));
        }

        public void RemoveSelectedEventTree()
        {
            var currentIndex = EventTrees.IndexOf(selectedEventTree);

            EventTrees.Remove(SelectedEventTreeFiltered);
            OnPropertyChanged(nameof(EventTrees));
            OnPropertyChanged(nameof(EventTreesToEstimate));

            if (currentIndex == -1 || currentIndex == EventTrees.Count)
            {
                SelectedEventTreeFiltered = EventTrees.LastOrDefault();
            }
            else
            {
                SelectedEventTreeFiltered = EventTrees.ElementAt(currentIndex);
            }
        }

        private void EventTreesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var eventTree in e.OldItems.OfType<EventTree>())
                {
                    var eventTreeViewModel = EventTrees.First(et => et.IsViewModelFor(eventTree));
                    eventTreeViewModel.PropertyChanged -= EventTreeViewModelPropertyChanged;
                    EventTrees.Remove(eventTreeViewModel);
                }
                
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var eventTree in e.NewItems.OfType<EventTree>())
                {
                    var eventTreeViewModel = new EventTreeViewModel(eventTree, projectManipulationService)
                    {
                        EstimationSpecificationViewModelFactory = new EstimationSpecificationViewModelFactory(Project)
                    };
                    eventTreeViewModel.PropertyChanged += EventTreeViewModelPropertyChanged;
                    EventTrees.Add(eventTreeViewModel);
                }

            }
        }

        private void HydraulicsViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.OfType<HydraulicConditionViewModel>())
                {
                    projectManipulationService.AddHydraulicCondition(item.HydraulicCondition);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<HydraulicConditionViewModel>())
                {
                    projectManipulationService.RemoveHydraulicCondition(item.HydraulicCondition);
                }
            }
        }

        private void ExpertViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.OfType<ExpertViewModel>())
                {
                    projectManipulationService.AddExpert(item.Expert);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<ExpertViewModel>())
                {
                    projectManipulationService.RemoveExpert(item.Expert);
                }
            }
        }


        private void EventTreeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is EventTreeViewModel))
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(EventTreeViewModel.MainTreeEventViewModel):
                    OnPropertyChanged(nameof(SelectedTreeEvent));
                    addTreeEventCommand.FireCanExecuteChanged();
                    removeTreeEventCommand.FireCanExecuteChanged();
                    break;
                case nameof(EventTreeViewModel.SelectedTreeEvent):
                    OnPropertyChanged(nameof(SelectedTreeEvent));
                    addTreeEventCommand.FireCanExecuteChanged();
                    removeTreeEventCommand.FireCanExecuteChanged();
                    break;
                case nameof(EventTreeViewModel.NeedsSpecification):
                    OnPropertyChanged(nameof(EventTreesToEstimate));
                    OnPropertyChanged(nameof(SelectedEventTreeUnFiltered));
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}