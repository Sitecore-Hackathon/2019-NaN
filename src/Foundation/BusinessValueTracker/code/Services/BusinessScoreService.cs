using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.ContentTesting.ML.Workers;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class BusinessScoreService : BaseWorker, IBusinessScoreService
    {
        private readonly ILogger<BusinessScoreService> _logger;
        private readonly ITableStoreFactory _tableStoreFactory;
        private readonly RfmCalculateService _rfmCalculateService;
        private readonly IServiceProvider _serviceProvider;

        private static List<Client> _clients;

        public BusinessScoreService(ITableStoreFactory tableStoreFactory, ILogger<BusinessScoreService> logger, IServiceProvider serviceProvider)
            : base(tableStoreFactory)
        {
            _tableStoreFactory = tableStoreFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rfmCalculateService = new RfmCalculateService();
        }

        public async Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables)
        {
            _logger.LogInformation("Executing Train method for table with schema=" + schemaName);

            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);

            var calculatedScores = _rfmCalculateService.CalculateRfmScores(ClientMapper.MapToClients(data));

            _clients = calculatedScores;

            await UpdateRfmFacet(_clients, cancellationToken);

            return new RfmStatistics {Clients = calculatedScores};
        }


        public async Task UpdateRfmFacet(List<Client> clients, CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                using (var xdbContext = scope.ServiceProvider.GetRequiredService<IXdbContext>())
                {
                    foreach (var client in clients)
                    {
                        var identifier = client.ClientId;

                        var reference = new IdentifiedContactReference(Constants.XConnect.IdentificationSource, identifier.ToString());
                        var contact = await xdbContext.GetContactAsync(reference, new ContactExpandOptions(
                            PersonalInformation.DefaultFacetKey,
                            EmailAddressList.DefaultFacetKey,
                            ContactBehaviorProfile.DefaultFacetKey,
                            RfmContactFacet.DefaultFacetKey
                        ));

                        if (contact != null)
                        {
                            var rfmFacet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey) ?? new RfmContactFacet();

                            rfmFacet.R = client.R;
                            rfmFacet.F = client.F;
                            rfmFacet.M = client.M;
                            rfmFacet.Recency = client.Recency;
                            rfmFacet.Frequency = client.Frequency;
                            rfmFacet.Monetary = (double)client.Monetary;


                            xdbContext.SetFacet(contact, RfmContactFacet.DefaultFacetKey, rfmFacet);

                            _logger.LogInformation($"RFM info: customerId={identifier}, R={rfmFacet.R}, F={rfmFacet.F}, M={rfmFacet.M}, Recency={rfmFacet.Recency}, Frequency={rfmFacet.Frequency}, Monetary={rfmFacet.Monetary}, CLUSTER={rfmFacet.Cluster}");

                        }
                    }

                    await xdbContext.SubmitAsync(cancellationToken);
                }
            }
        }

        public async Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables)
        {
            return new List<object>();
        }
    }

    public interface IBusinessScoreService
    {
        Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables);

        Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables);
    }
}