﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using Forest.Data;
using Forest.Data.Services;
using Forest.Data.Tree;
using Forest.Gui;
using Forest.Visualization.Commands;
using Forest.Visualization.DataTemplates.MainContentPresenter.EventTree;

namespace Forest.Visualization.ViewModels.ContentPanel.MainContentPresenter.EventTreeEditing
{
    public class EventTreeMainContentViewModel : Entity
    {
        private readonly CommandFactory commandFactory;
        private readonly EventTree eventTree;
        private readonly ForestGui gui;
        private readonly ViewModelFactory viewModelFactory;
        private EventTreeGraph graph;

        public EventTreeMainContentViewModel(EventTree eventTree, ForestGui gui)
        {
            this.gui = gui;
            commandFactory = new CommandFactory(gui);
            gui.PropertyChanged += GuiPropertyChanged;
            this.eventTree = eventTree;
            this.eventTree.PropertyChanged += EventTreePropertyChanged;
            this.eventTree.TreeEventsChanged += TreeEventsChanged;
            this.gui.SelectionManager.SelectedTreeEventChanged += SelectedTreeEventChanged;
            viewModelFactory = new ViewModelFactory(gui);
        }

        public EventTreeGraphLayout EventTreeGraphLayout => new EventTreeGraphLayout { Graph = CreateGraph() };

        public bool IsDetailsPanelVisible => gui.IsDetailsPanelVisible;

        public TreeEventViewModel SelectedTreeEventViewModel
        {
            get
            {
                var selectedTreeEvent = gui.SelectionManager.GetSelectedTreeEvent(eventTree);
                return selectedTreeEvent != null ? viewModelFactory.CreateTreeEventViewModel(selectedTreeEvent, eventTree) : null;
            }
        }

        public bool IsSaveToImage
        {
            get => gui.IsSaveToImage;
            set
            {
                gui.IsSaveToImage = value;
                gui.OnPropertyChanged();
            }
        }

        public ICommand CanvasClickedCommand => commandFactory.CreateClearTreeEventSelectionCommand(eventTree);

        public string Name
        {
            get => eventTree.Name;
            set
            {
                eventTree.Name = value;
                eventTree.OnPropertyChanged();
            }
        }

        private void EventTreePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(EventTree.MainTreeEvent):
                    OnPropertyChanged(nameof(EventTreeGraphLayout));
                    break;
            }
        }

        private void SelectedTreeEventChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedTreeEventViewModel));
        }

        private void TreeEventsChanged(object sender, TreeEventsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(EventTreeGraphLayout));
        }

        private EventTreeGraph CreateGraph()
        {
            graph = new EventTreeGraph();

            DrawNode(viewModelFactory.CreateTreeEventViewModel(eventTree.MainTreeEvent, eventTree));

            return graph;
        }

        private void DrawNode(TreeEventViewModel treeEventViewModel, GraphVertex parent = null)
        {
            if (treeEventViewModel == null)
                return;

            var vertex = new GraphVertex(treeEventViewModel);
            graph.AddVertex(vertex);
            if (parent != null)
                graph.AddEdge(new TreeEventConnector(parent, vertex));

            if (treeEventViewModel.FailingEvent != null)
            {
                DrawNode(treeEventViewModel.FailingEvent, vertex);
            }
            else
            {
                var lastVertex = new GraphVertex(false);
                graph.AddVertex(lastVertex);
                graph.AddEdge(new TreeEventConnector(vertex, lastVertex));
            }

            if (treeEventViewModel.PassingEvent != null)
            {
                DrawNode(treeEventViewModel.PassingEvent, vertex);
            }
            else
            {
                var lastVertex = new GraphVertex(true);
                graph.AddVertex(lastVertex);
                graph.AddEdge(new TreeEventConnector(vertex, lastVertex));
            }
        }

        private void GuiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ForestGui.IsDetailsPanelVisible):
                    OnPropertyChanged(nameof(IsDetailsPanelVisible));
                    break;
                case nameof(ForestGui.IsSaveToImage):
                    OnPropertyChanged(nameof(IsSaveToImage));
                    break;
            }
        }
    }
}