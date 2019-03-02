using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Engine.ML;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Workers
{
    public class RfmTrainingWorker : IDeferredWorker
    {
        private readonly IModel<Interaction> _model;
        private readonly RfmTrainingWorkerOptionsDictionary _options;
        private readonly ITableStore _tableStore;
        private readonly ILogger<RfmTrainingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RfmTrainingWorker(
            ITableStoreFactory tableStoreFactory,
            IServiceProvider provider,
            ILogger<RfmTrainingWorker> logger,
            AllowedModelsDictionary modelsDictionary,
            RfmTrainingWorkerOptionsDictionary options,
            IServiceProvider serviceProvider)
        {

            _tableStore = tableStoreFactory.Create(options.SchemaName);
            _options = options;
            _logger = logger;
            _model = modelsDictionary.CreateModel<Interaction>(provider, options.ModelType, options.ModelOptions);
            _serviceProvider = serviceProvider;
        }

        public RfmTrainingWorker(
            ITableStoreFactory tableStoreFactory,
            IServiceProvider provider,
            ILogger<RfmTrainingWorker> logger,
            AllowedModelsDictionary modelsDictionary,
            IReadOnlyDictionary<string, string> options,
            IServiceProvider serviceProvider)
            : this(tableStoreFactory, provider, logger, modelsDictionary, RfmTrainingWorkerOptionsDictionary.Parse(options), serviceProvider)
        {
        }

        public async Task RunAsync(CancellationToken token)
        {
            _logger.LogWarning("RfmTrainingWorker.RunAsync");

            var tableNames = _options.TableNames;
            var tableStatisticsTasks = new List<Task<TableStatistics>>(tableNames.Count);
            foreach (string tableName in tableNames)
                tableStatisticsTasks.Add(_tableStore.GetTableStatisticsAsync(tableName, token));
            await Task.WhenAll(tableStatisticsTasks).ConfigureAwait(false);
            var tableDefinitionList = new List<TableDefinition>(tableStatisticsTasks.Count);
            for (int index = 0; index < tableStatisticsTasks.Count; ++index)
            {
                var result = tableStatisticsTasks[index].Result;
                if (result == null)
                    _logger.LogWarning(string.Format("Statistics data for {0} table could not be retrieved. It will not participate in model training.", tableNames[index]));
                else
                    tableDefinitionList.Add(result.Definition);
            }
            var modelStatistics = await _model.TrainAsync(_options.SchemaName, token, tableDefinitionList.ToArray()).ConfigureAwait(false);
            await UpdateRfmFacet(modelStatistics as RfmStatistics, token);
        }

        public async Task UpdateRfmFacet(RfmStatistics rfmStatistics, CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                using (var xdbContext = scope.ServiceProvider.GetRequiredService<IXdbContext>())
                {
                    if (rfmStatistics.Clients.Any())
                    {
                        foreach (var client in rfmStatistics.Clients)
                        {
                            var identifier = client.Email;

                            var reference = new IdentifiedContactReference(Constants.XConnect.IdentificationSource, identifier);
                            var contact = await xdbContext.GetContactAsync(reference, new ContactExpandOptions(
                                PersonalInformation.DefaultFacetKey,
                                EmailAddressList.DefaultFacetKey,
                                ContactBehaviorProfile.DefaultFacetKey,
                                RfmContactFacet.DefaultFacetKey
                            ));

                            if (contact != null)
                            {
                                if (!contact.IsKnown)
                                {
                                    var identifierToRemove = contact.Identifiers.FirstOrDefault(x => x.Source == Constants.XConnect.IdentificationSourceEmail);
                                    if (identifierToRemove == null)
                                    {
                                        var newIdentifier = new ContactIdentifier(Constants.XConnect.IdentificationSourceEmail, identifier, ContactIdentifierType.Known);
                                        xdbContext.AddContactIdentifier(contact, newIdentifier);
                                    }
                                }

                                var emailFacet = contact.GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);
                                if (emailFacet == null)
                                {
                                    var preferredEmail = new EmailAddress(identifier, true);
                                    var emails = new EmailAddressList(preferredEmail, "Demo");
                                    xdbContext.SetEmails(contact, emails);
                                }


                                var rfmFacet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey) ?? new RfmContactFacet();

                                rfmFacet.R = client.R;
                                rfmFacet.F = client.F;
                                rfmFacet.M = client.M;
                                rfmFacet.Recency = client.Recency;
                                rfmFacet.Frequency = client.Frequency;
                                rfmFacet.Monetary = (double)client.Monetary;

                                xdbContext.SetFacet(contact, RfmContactFacet.DefaultFacetKey, rfmFacet);

                                _logger.LogInformation($"RFM info: customer={identifier}, R={rfmFacet.R}, F={rfmFacet.F}, M={rfmFacet.M}, Recency={rfmFacet.Recency}, Frequency={rfmFacet.Frequency}, Monetary={rfmFacet.Monetary}");

                            }

                            await xdbContext.SubmitAsync(cancellationToken);
                        }
                    } 
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}