using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections;
using Sitecore.ContentTesting.ML.Workers;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class BusinessScoreService : BaseWorker, IBusinessScoreService
    {
        private readonly ITableStoreFactory _tableStoreFactory;
        private readonly RfmCalculateService _rfmCalculateService;

        public BusinessScoreService(ITableStoreFactory tableStoreFactory)
            : base(tableStoreFactory)
        {
            _tableStoreFactory = tableStoreFactory;
            _rfmCalculateService = new RfmCalculateService();
        }

        public async Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables)
        {
            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);

            var calculatedScores = _rfmCalculateService.CalculateRfmScores(ClientMapper.MapToClients(data));

            return new RfmStatistics {Clients = calculatedScores};
        }
    }

    public interface IBusinessScoreService
    {
        Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken,
            params TableDefinition[] tables);

    }
}