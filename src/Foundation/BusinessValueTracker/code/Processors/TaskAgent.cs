using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Workers;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Tasks.Options;
using Sitecore.Processing.Tasks.Options.DataSources.DataExtraction;
using Sitecore.Processing.Tasks.Options.Workers.ML;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Processors
{
    public class TaskAgent
    {
        public virtual void Process(PipelineArgs args)
        {
            RegisterAll();
        }

        public async Task RegisterAll()
        {
            try
            {
                var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

                var dataSourceOptions = new InteractionDataSourceOptionsDictionary(new InteractionExpandOptions(IpInfo.DefaultFacetKey), 5, 10);
                var modelTrainingOptions = new ModelTrainingTaskOptions(typeof(GoalsProjectionModel).AssemblyQualifiedName, typeof(Interaction).AssemblyQualifiedName, new Dictionary<string, string> { ["TestCaseId"] = "Id" }, Constants.DemoGoal.ProjectionTableName, Constants.DemoGoal.ProjectionResultTableName);

                var x = await taskManager.RegisterRfmModelTrainingTaskChainAsync(modelTrainingOptions, dataSourceOptions, TimeSpan.FromDays(1));
                Log.Info("TaskAgent RegisterAll taskId=" + x, this);
            }
            catch (Exception ex)
            {
                Log.Error("TaskAgent RegisterAll exception", ex, this);
            }

        }

        public async Task RegisterProjectionWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            var modelOptions = new Dictionary<string, string> { ["TestCaseId"] = "Id" };
            var dictionary = new InteractionProjectionWorkerOptionsDictionary(
                typeof(GoalsProjectionModel).AssemblyQualifiedName, TimeSpan.MaxValue, "PurchaseOutcome",
                modelOptions);

            Log.Info("TaskAgent RegisterProjectionWorkerTask SchemaName=" + dictionary.SchemaName, this);


            Guid taskId = await taskManager.RegisterDistributedTaskAsync(
                new InteractionDataSourceOptionsDictionary(new InteractionExpandOptions(), 5, 10),
                dictionary,
                null,
                new TimeSpan(0, 10, 0)).ConfigureAwait(false);

            Log.Info("TaskAgent RegisterProjectionWorkerTask taskId=" + taskId, this);

            List<Task<Guid>> mergeTasks = new List<Task<Guid>>();

            MergeWorkerOptionsDictionary optionsDictionary1 = new MergeWorkerOptionsDictionary("PurchaseOutcome", "xxx_", TimeSpan.FromHours(2), dictionary.SchemaName);
            MergeWorkerOptionsDictionary optionsDictionary2 = optionsDictionary1;
            Guid[] guidArray = new Guid[1] { taskId };
            await taskManager.RegisterDeferredTaskAsync(optionsDictionary2, guidArray, new TimeSpan(0, 10, 0))
                .ConfigureAwait(false);

            var t = await taskManager.RegisterDeferredTaskAsync(
                new TrainingWorkerOptionsDictionary(
                    typeof(Interaction).AssemblyQualifiedName,
                    typeof(GoalsProjectionModel).AssemblyQualifiedName,
                    dictionary.SchemaName,
                    new List<string> { "PurchaseOutcome" },
                    modelOptions),

                new Guid[1] { taskId },
                new TimeSpan(0, 10, 0)).ConfigureAwait(false);


            Log.Info("TaskAgent TrainingWorkerOptionsDictionary taskId=" + t, this);
        }

        public async Task RegisterMergingWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            Guid taskId = await taskManager.RegisterDeferredTaskAsync(
                    new MergeWorkerOptionsDictionary(nameof(DemoGoal), "sctest_", TimeSpan.FromHours(2), "MergingWorkerSchema"),
                    null,
                    new TimeSpan(0, 5, 0))
                .ConfigureAwait(false);
            Sitecore.Diagnostics.Log.Info("TaskAgent RegisterMergingWorkerTask taskId=" + taskId, this);
        }

        public async Task RegisterTrainingWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            Guid taskId = await taskManager.RegisterDeferredTaskAsync(
                    new InteractionTrainingWorkerOptionsDictionary(typeof(DemoGoal).AssemblyQualifiedName, "ProjectionWorkerSchema", new List<string> { nameof(DemoGoal) }, new Dictionary<string, string> { ["TestCaseId"] = "Id2" }),
                    null,
                    new TimeSpan(0, 10, 0))
                .ConfigureAwait(false);
            Sitecore.Diagnostics.Log.Info("TaskAgent RegisterTrainingWorkerTask taskId=" + taskId, this);
        }

     
    }

    public static class TaskManagerExtensionsCustom
    {
        public static Task<Guid> RegisterRfmModelTrainingTaskChainAsync(
          this ITaskManager taskManager,
          ModelTrainingTaskOptions modelTrainingOptions,
          IDataSourceOptionsCollection dataSourceOptions,
          TimeSpan expiresAfter)
        {
            return taskManager.RegisterRfmModelTrainingTaskChainAsync(modelTrainingOptions, dataSourceOptions, expiresAfter, Enumerable.Empty<Guid>());
        }

        public static async Task<Guid> RegisterRfmModelTrainingTaskChainAsync(
          this ITaskManager taskManager,
          ModelTrainingTaskOptions modelTrainingOptions,
          IDataSourceOptionsCollection dataSourceOptions,
          TimeSpan expiresAfter,
          IEnumerable<Guid> prerequisiteTaskIds)
        {
            var projectionDictionary = new ProjectionWorkerOptionsDictionary(
                modelTrainingOptions.ModelEntityTypeString,
                modelTrainingOptions.ModelTypeString, expiresAfter, modelTrainingOptions.SchemaName,
                modelTrainingOptions.ModelOptions);

            Guid guid = await taskManager.RegisterDistributedTaskAsync(dataSourceOptions, projectionDictionary, prerequisiteTaskIds, expiresAfter).ConfigureAwait(false);
            List<Task<Guid>> mergeTasks = new List<Task<Guid>>();
            foreach (var targetTableNames in modelTrainingOptions.SourceTargetTableNamesMap)
            {
                var optionsDictionary1 = new MergeWorkerOptionsDictionary(targetTableNames.Value, targetTableNames.Key, expiresAfter, modelTrainingOptions.SchemaName);
                ITaskManager taskManager1 = taskManager;
                var optionsDictionary2 = optionsDictionary1;
                Guid[] guidArray = new Guid[1] { guid };
                TimeSpan expiresAfter1 = expiresAfter;
                mergeTasks.Add(taskManager1.RegisterDeferredTaskAsync(optionsDictionary2, guidArray, expiresAfter1));
            }
            Guid[] guidArray1 = await Task.WhenAll(mergeTasks).ConfigureAwait(false);
            var trainTaskId = await taskManager.RegisterDeferredTaskAsync(new RfmTrainingWorkerOptionsDictionary(modelTrainingOptions.ModelEntityTypeString, modelTrainingOptions.ModelTypeString, modelTrainingOptions.SchemaName, modelTrainingOptions.SourceTargetTableNamesMap.Values.ToList(), modelTrainingOptions.ModelOptions), mergeTasks.Select(t => t.Result), expiresAfter).ConfigureAwait(false);
            return trainTaskId;
        }
    }
}
