using System;
using Sitecore.XConnect;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets
{
    [Serializable]
    [FacetKey(DefaultFacetKey)]
    public class RfmContactFacet : Facet
    {
        public const string DefaultFacetKey = "RfmContactFacet";

        public int R { get; set; }
        public int F { get; set; }
        public int M { get; set; }
        public double Recency { get; set; }
        public int Frequency { get; set; }
        public double Monetary { get; set; }

        public int Cluster { get; set; }
    }
}