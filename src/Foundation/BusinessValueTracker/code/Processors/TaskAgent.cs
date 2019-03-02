using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
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
            //  RegisterProjectionWorkerTask();
        }

        //public async Task RegisterContactProjection()
        //{
        //    Log.Info("Cortex Try RegisterContactProjection", this);
        //    var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();
        //    var dataSourceOptions = new ContactDataSourceOptionsDictionary(new ContactExpandOptions(PersonalInformation.DefaultFacetKey,
        //        EmailAddressList.DefaultFacetKey,
        //        ContactBehaviorProfile.DefaultFacetKey)
        //        , 5, 10);

        //    var modelTrainingOptions =
        //        new ModelTrainingTaskOptions(typeof(ContactModel).AssemblyQualifiedName, typeof(Contact).AssemblyQualifiedName, new Dictionary<string, string> { ["TestCaseId"] = "Id" }, "ContactModel", "DemoContactResultTable");

        //    Log.Info("Cortex SchemaName=" + modelTrainingOptions.SchemaName, this);
        //    foreach (KeyValuePair<string, string> targetTableNames in modelTrainingOptions.SourceTargetTableNamesMap)
        //    {
        //        Log.Info("Cortex TableName=" + targetTableNames.Value + " prefix=" + targetTableNames.Key, this);
        //    }

        //    var x = await taskManager.RegisterModelTrainingTaskChainAsync(modelTrainingOptions, dataSourceOptions, TimeSpan.FromDays(1));
        //    Sitecore.Diagnostics.Log.Info("Cortex RegisterAll taskId=" + x, this);
        //}

        public async Task RegisterAll()
        {
            try
            {
                Log.Info("Cortex Try RegisterAll", this);
                var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

                var dataSourceOptions = new InteractionDataSourceOptionsDictionary(new InteractionExpandOptions(IpInfo.DefaultFacetKey), 5, 10);
                var modelTrainingOptions = new ModelTrainingTaskOptions(typeof(GoalsProjectionModel).AssemblyQualifiedName, typeof(Interaction).AssemblyQualifiedName, new Dictionary<string, string> { ["TestCaseId"] = "Id" }, Constants.DemoGoal.ProjectionTableName, Constants.DemoGoal.ProjectionResultTableName);

                var x = await taskManager.RegisterRfmModelTrainingTaskChainAsync(modelTrainingOptions, dataSourceOptions, TimeSpan.FromDays(1));
                Log.Info("Cortex RegisterAll taskId=" + x, this);
            }
            catch (Exception ex)
            {
                Log.Error("Cortex RegisterAll exception", ex, this);
            }

        }

        public async Task RegisterProjectionWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            var modelOptions = new Dictionary<string, string> { ["TestCaseId"] = "Id" };
            var dictionary = new InteractionProjectionWorkerOptionsDictionary(
                typeof(GoalsProjectionModel).AssemblyQualifiedName, TimeSpan.MaxValue, "PurchaseOutcome",
                modelOptions);

            Log.Info("Cortex RegisterProjectionWorkerTask SchemaName=" + dictionary.SchemaName, this);


            Guid taskId = await taskManager.RegisterDistributedTaskAsync(
                new InteractionDataSourceOptionsDictionary(new InteractionExpandOptions(), 5, 10),
                dictionary,
                null,
                new TimeSpan(0, 10, 0)).ConfigureAwait(false);

            Log.Info("Cortex RegisterProjectionWorkerTask taskId=" + taskId, this);

            List<Task<Guid>> mergeTasks = new List<Task<Guid>>();

            MergeWorkerOptionsDictionary optionsDictionary1 = new MergeWorkerOptionsDictionary("PurchaseOutcome", "xxx_", TimeSpan.FromHours(2), dictionary.SchemaName);
            // ITaskManager taskManager1 = taskManager;
            MergeWorkerOptionsDictionary optionsDictionary2 = optionsDictionary1;
            Guid[] guidArray = new Guid[1] { taskId };
            //mergeTasks.Add(taskManager1.RegisterDeferredTaskAsync(optionsDictionary2, guidArray, new TimeSpan(0, 10, 0)));
            await taskManager.RegisterDeferredTaskAsync(optionsDictionary2, guidArray, new TimeSpan(0, 10, 0))
                .ConfigureAwait(false);

            //Guid[] guidArray1 = await Task.WhenAll<Guid>(mergeTasks).ConfigureAwait(false);
            var t = await taskManager.RegisterDeferredTaskAsync(
                new TrainingWorkerOptionsDictionary(
                    typeof(Interaction).AssemblyQualifiedName,
                    typeof(GoalsProjectionModel).AssemblyQualifiedName,
                    dictionary.SchemaName,
                    new List<string> { "PurchaseOutcome" },
                    modelOptions),

                new Guid[1] { taskId },
                new TimeSpan(0, 10, 0)).ConfigureAwait(false);


            Log.Info("Cortex TrainingWorkerOptionsDictionary taskId=" + t, this);

            //Guid taskId2 = await taskManager1.RegisterDeferredTaskAsync(
            //        new MergeWorkerOptionsDictionary("PurchaseOutcome", "", TimeSpan.FromHours(2), "MergeSchema"),
            //        new Guid[] { taskId },
            //        new TimeSpan(0, 10, 0))
            //    .ConfigureAwait(false);
            //Log.Info("Cortex RegisterMergingWorkerTask taskId=" + taskId2, this);

            //Guid deferredTaskId = await taskManager.RegisterDeferredTaskAsync(
            //    new DeferredWorkerOptionsDictionary("Sitecore.Documentation.SampleDeferredWorker",
            //        new Dictionary<string, string>() { { "testkey", "testvalue" } }),
            //    null,
            //    TimeSpan.FromDays(1));

            //Guid taskId2 = await taskManager.RegisterDistributedTaskAsync(
            //    new ContactDataSourceOptionsDictionary(new ContactExpandOptions(), 5, 10),
            //    new ContactProjectionWorkerOptionsDictionary("", TimeSpan.MaxValue, "SampleSchemaName", new Dictionary<string, string> { ["TestCaseId"] = "Id" }),
            //    null,
            //    new TimeSpan(0, 10, 0));

            //List<string> list = new List<string>{"TestTable"};
            //var contactTrainingWorkerOptionsDictionary = new ContactTrainingWorkerOptionsDictionary("",
            //    "ContactTrainingWorkerSchemaName", list, new Dictionary<string, string> {["TestCaseId"] = "Id"});

            //Guid taskId2 = await taskManager.RegisterDeferredTaskAsync(
            ////    new ContactDataSourceOptionsDictionary(new ContactExpandOptions(), 5, 10),
            //    contactTrainingWorkerOptionsDictionary,
            //    null,
            //    new TimeSpan(0, 5, 0));

        }

        public async Task RegisterMergingWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            Guid taskId = await taskManager.RegisterDeferredTaskAsync(
                    new MergeWorkerOptionsDictionary(nameof(DemoGoal), "sctest_", TimeSpan.FromHours(2), "MergingWorkerSchema"),
                    null,
                    new TimeSpan(0, 5, 0))
                .ConfigureAwait(false);
            Sitecore.Diagnostics.Log.Info("Cortex RegisterMergingWorkerTask taskId=" + taskId, this);
        }

        public async Task RegisterTrainingWorkerTask()
        {
            var taskManager = ServiceLocator.ServiceProvider.GetService<ITaskManager>();

            Guid taskId = await taskManager.RegisterDeferredTaskAsync(
                    new InteractionTrainingWorkerOptionsDictionary(typeof(DemoGoal).AssemblyQualifiedName, "ProjectionWorkerSchema", new List<string> { nameof(DemoGoal) }, new Dictionary<string, string> { ["TestCaseId"] = "Id2" }),
                    null,
                    new TimeSpan(0, 10, 0))
                .ConfigureAwait(false);
            Sitecore.Diagnostics.Log.Info("Cortex RegisterTrainingWorkerTask taskId=" + taskId, this);
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
            //var dataSourceOptions2 = new ContactDataSourceOptionsDictionary(new ContactExpandOptions(PersonalInformation.DefaultFacetKey,
            //        EmailAddressList.DefaultFacetKey,
            //        ContactBehaviorProfile.DefaultFacetKey,
            //        RfmContactFacet.DefaultFacetKey)
            //    , 5, 10);

            //var modelTrainingOptions2 =
            //    new ModelTrainingTaskOptions(typeof(ContactProjectionModel).AssemblyQualifiedName, typeof(Contact).AssemblyQualifiedName, new Dictionary<string, string> { ["TestCaseId"] = "Id" }, Constants.DemoGoal.ProjectionContactTableName, Constants.DemoGoal
            //        .ProjectionContactResultTableName);


            //var evaluationDictionary = new EvaluationWorkerOptionsDictionary(
            //    "Hackathon.Boilerplate.Foundation.BusinessValueTracker.RfmEvaluationWorker, Hackathon.Boilerplate.Foundation.BusinessValueTracker",
            //    modelTrainingOptions2.ModelTypeString, modelTrainingOptions2.ModelOptions,
            //    "Evaluator.Schema", TimeSpan.FromDays(1));


            //return await taskManager.RegisterDistributedTaskAsync(
            //        dataSourceOptions2,
            //        evaluationDictionary,
            //        new[] { trainTaskId },
            //        expiresAfter)
            //    .ConfigureAwait(false);


        }
    }
}
