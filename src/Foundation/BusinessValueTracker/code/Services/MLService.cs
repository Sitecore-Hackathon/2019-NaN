using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class MLService : BaseWorker, IMLService
    {
        private readonly ITableStoreFactory _tableStoreFactory;
        private readonly ILogger<MLService> _logger;

        private readonly double _testValue;
        private RfmCalculateService _rfmCalculateService;

        public Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            _logger.LogInformation("Executing Train method of MLNetService for table with schema=" + schemaName);
            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);

            var calculatedScores = _rfmCalculateService.CalculateRfmScores(ClientMapper. MapToClients(data));

            var businessData = calculatedScores.Select(x => new Rfm
            {
                R = x.R,
                F = x.F,
                M = x.M
            }).ToList();

            return new RfmStatistics { Clients = calculatedScores };
        }

        public Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMLService
    {
        Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables);

        Task<IReadOnlyList<object>> Evaluate(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables);
    }
}