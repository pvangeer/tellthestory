﻿using System;
using Forest.Data;
using Forest.Storage.XmlEntities;

namespace Forest.Storage.Create
{
    internal static class ProjectCreateExtensions
    {
        internal static ForestAnalysisXmlEntity Create(this ForestAnalysis forestAnalysis, PersistenceRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            var entity = new ForestAnalysisXmlEntity
            {
                Name = forestAnalysis.Name.DeepClone(),
                Description = forestAnalysis.Description.DeepClone(),
                AssessmentSection = forestAnalysis.AssessmentSection.DeepClone(),
                ProjectInformation = forestAnalysis.ProjectInformation.DeepClone(),
                ProjectLeader = forestAnalysis.ProjectLeader.Create(registry),
                EventTree = forestAnalysis.EventTree.Create(registry)
            };

            return entity;
        }
    }
}