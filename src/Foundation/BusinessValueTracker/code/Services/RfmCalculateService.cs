using System;
using System.Collections.Generic;
using System.Linq;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Extentions;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Services
{
    public class RfmCalculateService
    {
        public List<Client> CalculateRfmScores(List<Client> list)
        {
            // Calculate pre-values

            foreach (Client customer in list)
            {
                // Monetary

                decimal m = 0;
                foreach (ClientEvent clientEvent in customer.ClientEvents)
                {
                    m = m + Math.Abs(clientEvent.Value);
                }

                if (m <= 0) // broken data
                    m = 1;

                customer.Monetary = m;

                // Recency

                DateTime? minDate = customer.ClientEvents.Where(x => x.TimeStamp.Year != 1).DefaultIfEmpty().Min(x => x?.TimeStamp);
                DateTime? maxDate = customer.ClientEvents.Where(x => x.TimeStamp.Year != 1).DefaultIfEmpty().Max(x => x?.TimeStamp);

                customer.Recency = minDate.HasValue && maxDate.HasValue ? (maxDate - minDate).Value.TotalDays + 1 : 1;

                // Frequency

                customer.Frequency = customer.ClientEvents.Count;

                if (customer.Frequency == 0) // broken data
                    customer.Frequency = 1;
            }

            // Calculate 

            int maxScore = 3;

            var rList = list.OrderByDescending(x => x.Recency).ToList().Partition(maxScore);
            int rValue = maxScore;
            foreach (var rPart in rList)
            {
                foreach (Client customer in rPart)
                {
                    customer.R = rValue;
                }

                rValue = rValue - 1;
            }

            var mList = list.OrderByDescending(x => x.Monetary).ToList().Partition(maxScore);

            int mValue = maxScore;
            foreach (var mPart in mList)
            {
                foreach (Client customer in mPart)
                {
                    customer.M = mValue;
                }

                mValue = mValue - 1;
            }

            var fList = list.OrderByDescending(x => x.Frequency).ToList().Partition(maxScore);
            int fValue = maxScore;
            foreach (var fPart in fList)
            {
                foreach (Client customer in fPart)
                {
                    customer.F = fValue;
                }

                fValue = fValue - 1;
            }

            return list;

        }
    }
}
