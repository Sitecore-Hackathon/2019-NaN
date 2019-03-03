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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

                var x = await taskManager.RegisterRfmModelTrainingTaskChainAsync(modelTrainingOptions, dataSourceOptions, TimeSpan.FromHours(5));
                Log.Info("TaskAgent RegisterAll taskId=" + x, this);
            }
            catch (Exception ex)
            {
                Log.Error("TaskAgent RegisterAll exception", ex, this);
            }

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
            var mergeTasks = new List<Task<Guid>>();
            foreach (var targetTableNames in modelTrainingOptions.SourceTargetTableNamesMap)
            {
                var optionsDictionary1 = new MergeWorkerOptionsDictionary(targetTableNames.Value, targetTableNames.Key, expiresAfter, modelTrainingOptions.SchemaName);
                ITaskManager taskManager1 = taskManager;
                var optionsDictionary2 = optionsDictionary1;
                Guid[] guidArray = { guid };
                TimeSpan expiresAfter1 = expiresAfter;
                mergeTasks.Add(taskManager1.RegisterDeferredTaskAsync(optionsDictionary2, guidArray, expiresAfter1));
            }
            await Task.WhenAll(mergeTasks).ConfigureAwait(false);
            var trainTaskId = await taskManager.RegisterDeferredTaskAsync(new RfmTrainingWorkerOptionsDictionary(modelTrainingOptions.ModelEntityTypeString, modelTrainingOptions.ModelTypeString, modelTrainingOptions.SchemaName, modelTrainingOptions.SourceTargetTableNamesMap.Values.ToList(), modelTrainingOptions.ModelOptions), mergeTasks.Select(t => t.Result), expiresAfter).ConfigureAwait(false);
            return trainTaskId;
        }
    }
}
