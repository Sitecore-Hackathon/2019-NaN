using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.XConnect;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections
{
    public class GoalsProjectionModel : IModel<Interaction>
    {
        private readonly IBusinessScoreService _businessScoreService;

        public GoalsProjectionModel(IReadOnlyDictionary<string, string> options, IBusinessScoreService businessScoreService)
        {
            _businessScoreService = businessScoreService;

            Projection = Sitecore.Processing.Engine.Projection.Projection.Of<Interaction>()
                .CreateTabular(Constants.DemoGoal.ProjectionTableName,
                    interaction => interaction.Events.OfType<Goal>().Where(x => x.DefinitionId == DemoGoal.DemoGoalDefinitionId),
                    cfg => cfg.Key(Constants.DemoGoal.ProjectionKey, x => x.Id)
                        .Attribute(Constants.DemoGoal.ProjectionTimestamp, x => x.Timestamp)
                        .Attribute(Constants.DemoGoal.ProjectionEngagementValue, x => x.EngagementValue)
                        .Attribute(Constants.DemoGoal.CustomerIdKey, x => x.CustomValues[Constants.DemoGoal.CustomerIdKeyValue])
                );

           
        }

        public Task<ModelStatistics> TrainAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            return _businessScoreService.Train(schemaName, cancellationToken, tables);
        }

        public Task<IReadOnlyList<object>> EvaluateAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException("advertising space :) follow me in twitter @x3mxray");
        }

        public IProjection<Interaction> Projection { get; }
    }
}