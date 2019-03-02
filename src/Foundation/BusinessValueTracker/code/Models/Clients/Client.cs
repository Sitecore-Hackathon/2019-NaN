using System.Collections.Generic;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models
{
    public class Client : CustomerBusinessValue
    {
        public Client()
        {
            ClientEvents = new List<ClientEvent>();
        }

        public int ClientId { get; set; }
        public IList<ClientEvent> ClientEvents { get; set; }
    }
}