﻿using System;
using System.Linq;
using Forest.Data;
using Forest.Messaging;
using Forest.Storage;
using log4net;
using log4net.Repository.Hierarchy;

namespace Forest.Gui
{
    public class ForestGui : Entity, IMessageCollection
    {
        public ForestGui()
        {
            ConfigureMessaging();
            BusyIndicator = StorageState.Idle;
            Messages = new MessageList();
            ForestAnalysis = ForestAnalysisFactory.CreateStandardNewAnalysis();
            ProjectFilePath = "";
            GuiProjectServices = new GuiProjectServices(this);
            SelectionManager = new SelectionManager(this);
            IsShowDetailsPanel = true;
            LogMessageAppender.Instance.MessageCollection = this;
        }

        public ForestAnalysis ForestAnalysis { get; set; }

        public VersionInfo VersionInfo { get; set; }

        public StorageState BusyIndicator { get; set; }

        public bool IsSaveToImage { get; set; }

        public string ProjectFilePath { get; set; }

        public GuiProjectServices GuiProjectServices { get; }

        public SelectionManager SelectionManager { get; }

        public Func<ShouldProceedState> ShouldSaveOpenChanges { get; set; }

        public Func<bool> ShouldMigrateProject { get; set; }

        public MessageList Messages { get; }

        public bool IsShowDetailsPanel { get; set; }

        private void ConfigureMessaging()
        {
            var rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;

            if (!rootLogger.Appenders.Any(a => a is LogMessageAppender))
            {
                rootLogger.AddAppender(new LogMessageAppender());
                rootLogger.Repository.Configured = true;
            }
        }
    }
}