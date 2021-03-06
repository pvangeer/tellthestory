﻿using System;
using StoryTree.Data.Tree;
using StoryTree.Storage.DbContext;

namespace StoryTree.Storage.Create
{
    internal static class ExpertClassEstimationCreateExtensions
    {
        internal static ExpertClassEstimationEntity Create(this ExpertClassEstimation model, PersistenceRegistry registry)
        {
            var entity = new ExpertClassEstimationEntity
            {
                ExpertEntity = model.Expert.Create(registry),
                HydraulicConditionElementEntity = model.HydraulicCondition.Create(registry),
                AverageEstimation = Convert.ToByte(model.AverageEstimation),
                MaxEstimation = Convert.ToByte(model.MaxEstimation),
                MinEstimation = Convert.ToByte(model.MinEstimation),
                // TODO: Add Comment to data model and map
            };

            return entity;
        }
    }
}
