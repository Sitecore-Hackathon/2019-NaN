using System.Collections.Generic;
using System.Linq;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Sitecore.Processing.Engine.Projection;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Mappers
{
    public static class ContactMapper
    {
        public static bool Enabled(this IDataRow dataRow)
        {
            var email = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "Enabled");
            if (email != null)
            {
                return dataRow.GetInt64(dataRow.Schema.GetFieldIndex("Enabled")) == 1;
            }
            return false;
        }

        public static RfmContactFacet MapToRfmFacet(this IDataRow dataRow)
        {
            var result = new RfmContactFacet();

            var r = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "R");
            if (r != null)
            {
                result.R = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex("R"));
            }
            var f = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "F");
            if (f != null)
            {
                result.F = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex("F"));
            }
            var m = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "M");
            if (m != null)
            {
                result.M = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex("M"));
            }

            var recency = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "Recency");
            if (recency != null)
            {
                result.Recency = dataRow.GetDouble(dataRow.Schema.GetFieldIndex("Recency"));
            }
            var frequency = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "Frequency");
            if (frequency != null)
            {
                result.Frequency = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex("Frequency"));
            }
            var monetary = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "Monetary");
            if (monetary != null)
            {
                result.Monetary = dataRow.GetDouble(dataRow.Schema.GetFieldIndex("Monetary"));
            }
            return result;
        }
    }
}