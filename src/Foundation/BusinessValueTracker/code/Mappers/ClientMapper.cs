using System.Collections.Generic;
using System.Linq;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;
using Sitecore.Processing.Engine.Projection;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers
{
    public static class ClientMapper
    {
        public static List<Client> MapToClients(IReadOnlyList<IDataRow> dataRows)
        {
            var invoices = new List<ClientEvent>();
            foreach (var data in dataRows)
            {
                invoices.Add(data.ToGoalOutcome());
            }

            var groupedData = invoices.AsEnumerable().GroupBy(x => x.Email);

            return groupedData.Select(data => new Client { Email = data.Key, ClientEvents = data.ToList() }).ToList();
        }

        public static ClientEvent ToGoalOutcome(this IDataRow dataRow)
        {
            var result = new ClientEvent();
            var email = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == Constants.DemoGoal.CustomerIdKey);
            if (email != null)
            {
                result.Email = dataRow.GetString(dataRow.Schema.GetFieldIndex(Constants.DemoGoal.CustomerIdKey));
            }

            var date = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == Constants.DemoGoal.ProjectionTimestamp);
            if (date != null)
            {
                result.TimeStamp = dataRow.GetDateTime(dataRow.Schema.GetFieldIndex(Constants.DemoGoal.ProjectionTimestamp));
            }

            var price = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == Constants.DemoGoal.ProjectionEngagementValue);
            if (price != null)
            {
                result.Value = (decimal)dataRow.GetDouble(dataRow.Schema.GetFieldIndex(Constants.DemoGoal.ProjectionEngagementValue));
            }

            return result;
        }
    }
}
