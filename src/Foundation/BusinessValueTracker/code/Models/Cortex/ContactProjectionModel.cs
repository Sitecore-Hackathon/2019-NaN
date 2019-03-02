using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex
{
    public class ContactProjectionModel : IModel<Contact>
    {
        private readonly IMLService _mlNetService;
        private readonly ILogger<ContactProjectionModel> _logger;

        public ContactProjectionModel(IReadOnlyDictionary<string, string> options, IMLService mlNetService, ILogger<ContactProjectionModel> logger)
        {
            _logger = logger;
            _mlNetService = mlNetService;

           
            Projection = Sitecore.Processing.Engine.Projection.Projection.Of<Contact>().CreateTabular(
                Constants.DemoGoal.ProjectionContactTableName,
                cfg => cfg
                    .Key("ContactId", c => c.Id)
                    .Attribute("Enabled", c => c.GetFacet<RfmContactFacet>() == null ? 0 : 1)
                    .Attribute("R", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().R)
                    .Attribute("F", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().F)
                    .Attribute("M", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().M)
                    .Attribute("Recency", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Recency)
                    .Attribute("Frequency", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Frequency)
                    .Attribute("Monetary", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Monetary)
                    .Attribute("Email", c => c.Emails() == null ? "empty" : c.Emails().PreferredEmail == null ? "empty" : c.Emails().PreferredEmail.SmtpAddress));


        }

        public Task<ModelStatistics> TrainAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException("It`s a half of hackathone time :) ");
        }

        public Task<IReadOnlyList<object>> EvaluateAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            _logger.LogInformation("ContactModel -> EvaluateAsync");
            return _mlNetService.Evaluate(schemaName, cancellationToken, tables);
        }

        public IProjection<Contact> Projection { get; }



    }
}