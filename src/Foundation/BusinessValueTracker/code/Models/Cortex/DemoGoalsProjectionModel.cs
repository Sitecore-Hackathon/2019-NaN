using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.XConnect;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex
{
    public class DemoGoalsProjectionModel : IModel<Interaction>
    {
        private readonly IMLService _mlService;
        private readonly ILogger<DemoGoalsProjectionModel> _logger;

        public DemoGoalsProjectionModel(IReadOnlyDictionary<string, string> options, IMLService mlService, ILogger<DemoGoalsProjectionModel> logger)
        {
            _logger = logger;
            _mlService = mlService;

       
            Projection = Sitecore.Processing.Engine.Projection.Projection.Of<Interaction>()
                .CreateTabular(Constants.DemoGoal.ProjectionTableName,
                    interaction => interaction.Events.OfType<DemoGoal>(),
                    cfg => cfg.Key(Constants.DemoGoal.ProjectionKey, x => x.Id)
                        .Attribute(Constants.DemoGoal.ProjectionTimestamp, x => x.Timestamp)
                        .Attribute(Constants.DemoGoal.ProjectionEngagementValue, x => x.EngagementValue)
                        .Attribute(Constants.DemoGoal.CustomerIdKey, x => x.CustomValues["CustomerId"])
                );

           
        }

        public Task<ModelStatistics> TrainAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            return _mlService.Train(schemaName, cancellationToken, tables);
        }

        public Task<IReadOnlyList<object>> EvaluateAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException();
        }

        public IProjection<Interaction> Projection { get; }
    }
}