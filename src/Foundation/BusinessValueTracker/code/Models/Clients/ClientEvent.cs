using System;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients
{
    public class ClientEvent
    {
        public decimal Value { get; set; }
        public int ContactId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}