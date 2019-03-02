using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Processing.Engine.ML.Abstractions;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Cortex
{
    public class RfmStatistics : ModelStatistics
    {
        public List<Client> Clients { get; set; }
    }
}