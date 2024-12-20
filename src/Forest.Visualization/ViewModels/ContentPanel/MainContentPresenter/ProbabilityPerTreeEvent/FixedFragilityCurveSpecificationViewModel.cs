﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Forest.Data.Estimations.PerTreeEvent;
using Forest.Data.Probabilities;

namespace Forest.Visualization.ViewModels.ContentPanel.MainContentPresenter.ProbabilityPerTreeEvent
{
    public class FixedFragilityCurveSpecificationViewModel : ProbabilitySpecificationViewModelBase
    {
        private readonly ObservableCollection<FragilityCurveElementViewModel> fixedFragilityCurveViewModels;
        private readonly ProbabilityEstimationPerTreeEvent parentEstimation;

        public FixedFragilityCurveSpecificationViewModel(
            TreeEventProbabilityEstimate estimate,
            ProbabilityEstimationPerTreeEvent parentEstimation,
            ViewModelFactory factory) : base(estimate, factory)
        {
            this.parentEstimation = parentEstimation;

            fixedFragilityCurveViewModels =
                new ObservableCollection<FragilityCurveElementViewModel>(
                    estimate.FragilityCurve.Select(e => new FragilityCurveElementViewModel(e)));
            fixedFragilityCurveViewModels.CollectionChanged += FragilityCurveViewModelsCollectionChanged;
        }

        public ObservableCollection<FragilityCurveElementViewModel> FixedFragilityCurve => FixedFragilityCurveViewModels();

        private ObservableCollection<FragilityCurveElementViewModel> FixedFragilityCurveViewModels()
        {
            var estimatedWaterLevels = fixedFragilityCurveViewModels.Select(vm => vm.WaterLevel).ToArray();
            var currentWaterLevels = parentEstimation.HydrodynamicConditions.Select(hc => hc.WaterLevel).Distinct().OrderBy(w => w).ToArray();
            var missingWaterLevels = currentWaterLevels.Except(estimatedWaterLevels).ToArray();
            var waterLevelsToRemove = estimatedWaterLevels.Except(currentWaterLevels).ToArray();
            foreach (var waterLevel in waterLevelsToRemove)
            {
                fixedFragilityCurveViewModels.Remove(
                    fixedFragilityCurveViewModels.FirstOrDefault(vm => Math.Abs(vm.WaterLevel - waterLevel) < 1e-8));
            }

            foreach (var waterLevel in missingWaterLevels)
            {
                var firstElementWithHigherWater = fixedFragilityCurveViewModels.FirstOrDefault(vm => vm.WaterLevel > waterLevel);
                var indexOfFirstElementWithHigherWater = fixedFragilityCurveViewModels.IndexOf(firstElementWithHigherWater);
                fixedFragilityCurveViewModels.Insert(Math.Max(0, indexOfFirstElementWithHigherWater),
                    new FragilityCurveElementViewModel(new FragilityCurveElement(waterLevel, (Probability)1.0)));
            }

            return fixedFragilityCurveViewModels;
        }

        private void FragilityCurveViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (var item in e.NewItems.OfType<FragilityCurveElementViewModel>())
                    Estimate.FragilityCurve.Add(item.FragilityCurveElement);

            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (var item in e.OldItems.OfType<FragilityCurveElementViewModel>())
                    Estimate.FragilityCurve.Remove(item.FragilityCurveElement);
        }
    }
}