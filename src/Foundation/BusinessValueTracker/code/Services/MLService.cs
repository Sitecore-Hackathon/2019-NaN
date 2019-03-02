using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sitecore.ContentTesting.ML.Workers;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class MLService : BaseWorker, IMLService
    {
        private readonly ILogger<MLService> _logger;
        private readonly ITableStoreFactory _tableStoreFactory;
        private readonly RfmCalculateService _rfmCalculateService;
        private static List<Client> _clients;

        public MLService(ITableStoreFactory tableStoreFactory, IConfiguration configuration, ILogger<MLService> logger)
            : base(tableStoreFactory)
        {
            _tableStoreFactory = tableStoreFactory;
            _logger = logger;
            _rfmCalculateService = new RfmCalculateService();
        }

        public async Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables)
        {
            _logger.LogInformation("Executing Train method of MLNetService for table with schema=" + schemaName);

            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);

            var calculatedScores = _rfmCalculateService.CalculateRfmScores(ClientMapper.MapToClients(data));

            _clients = calculatedScores;

            return new RfmStatistics {Clients = calculatedScores};
        }

        public async Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables)
        {
            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);

            var results = new List<PredictionResult>();

            foreach (var dataRow in data)
            {
                if (dataRow.Enabled())
                {

                }
            }

            return results;
        }
    }

    public interface IMLService
    {
        Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables);

        Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables);
    }
}