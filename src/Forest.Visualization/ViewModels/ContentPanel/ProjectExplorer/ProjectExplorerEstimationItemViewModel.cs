﻿using System.Windows.Input;
using Forest.Data.Estimations;
using Forest.Data.Estimations.PerTreeEvent;
using Forest.Gui;
using Forest.Visualization.Commands;

namespace Forest.Visualization.ViewModels.ContentPanel.ProjectExplorer
{
    public class ProjectExplorerEstimationItemViewModel : ProjectExplorerItemViewModelBase
    {
        private readonly CommandFactory commandFactory;
        private readonly ProbabilityEstimationPerTreeEvent estimation;
        private readonly ForestGui gui;

        public ProjectExplorerEstimationItemViewModel(ProbabilityEstimationPerTreeEvent estimation, ForestGui gui) : base(
            new ViewModelFactory(gui))
        {
            this.estimation = estimation;
            this.gui = gui;
            commandFactory = new CommandFactory(gui);
        }

        public override ICommand SelectItemCommand => commandFactory.CreateSelectItemCommand(this);

        public override string DisplayName => estimation.Name;

        public override string IconSourceString =>
            "pack://application:,,,/Forest.Visualization;component/Resources/ProjectExplorer/probability_estimation.ico";

        public override ICommand RemoveItemCommand => commandFactory.CreateRemoveEstimationPerTreeEventCommand(estimation);

        public override object GetSelectableObject()
        {
            return estimation;
        }

        public override bool IsViewModelFor(object item)
        {
            return item is ProbabilityEstimation probability && probability == estimation;
        }
    }
}