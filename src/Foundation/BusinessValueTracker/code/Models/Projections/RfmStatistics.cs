using System.Collections.Generic;
using Sitecore.Processing.Engine.ML.Abstractions;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Projections
{
    public class RfmStatistics : ModelStatistics
    {
        public List<Client> Clients { get; set; }
    }
}