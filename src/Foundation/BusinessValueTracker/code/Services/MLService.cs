using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class MLService : IMLService
    {
        public Task<ModelStatistics> Train(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException();
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