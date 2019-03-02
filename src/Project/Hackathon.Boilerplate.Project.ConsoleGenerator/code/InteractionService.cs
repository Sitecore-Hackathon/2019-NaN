using CsvHelper;
using Sitecore.UniversalTrackerClient.Entities;
using Sitecore.UniversalTrackerClient.Request.RequestBuilder;
using Sitecore.UniversalTrackerClient.Session.SessionBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Boilerplate.Project.ConsoleGenerator
{
    class InteractionService
    {

        public string instanceUrl { get; set; } = "http://my.site.com1";

        internal async Task ImportFromFileAsync(string file)
        {
            Interaction[] interactions = null;
            using (TextReader reader = File.OpenText(file))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Read();
                if (csvReader.ReadHeader())
                {
                    csvReader.ValidateHeader<Interaction>();
                    interactions = csvReader.GetRecords<Interaction>().ToArray();
                }

            }

            Console.WriteLine("File has been read successfully");
            await PushInteractiosToUT(interactions);
        }

        private async Task PushInteractiosToUT(IEnumerable<Interaction> interactions)
        {
            string channelId = "27b4e611-a73d-4a95-b20a-811d295bdf65";
            string definitionId = "01f8ffbf-d662-4a87-beee-413307055c48";
            foreach (var interaction in interactions.GroupBy(x => x.CustomerId))
            {
                var events = interaction.Select(x => new UTEvent(x.Timestamp, new Dictionary<string, string>(),
                    "", "", x.GoalValue, "", "", null, "")).ToArray();
                var defaultInteractionQuery = UTEntitiesBuilder.Interaction()
                                                           .ChannelId(channelId)
                                                           .Initiator(InteractionInitiator.Contact)
                                                           .Contact("jsdemo", "demo");
                foreach (var e in events)
                {
                    defaultInteractionQuery.AddEvents(e);
                }

                var defaultInteraction = defaultInteractionQuery.Build();
                using (var session = SitecoreUTSessionBuilder.SessionWithHost(instanceUrl)
                                                        .DefaultInteraction(defaultInteraction)
                                                        .BuildSession()
                        )
                {
                    var eventRequest = UTRequestBuilder.EventWithDefenitionId(definitionId)
                                                       .Timestamp(DateTime.Now).Build();
                    var eventResponse = await session.TrackEventAsync(eventRequest);
                    Console.WriteLine("Track EVENT RESULT: " + eventResponse.StatusCode);
                }
                Console.WriteLine($"Customer {interaction.Key} imported");
            }
        }

        internal async Task GenerateInteractions(int customerId, int count)
        {
            var interactions = new List<Interaction>();
            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                var intr = new Interaction()
                {
                    CustomerId = customerId,
                    GoalValue = rand.Next(0, 20),
                    GoalId = ((GoalType)rand.Next(0, Enum.GetNames(typeof(GoalType)).Length)).ToString(),
                    Timestamp = DateTime.UtcNow.AddDays(rand.Next(-50, 50)),
                };
                interactions.Add(intr);
            }

            await PushInteractiosToUT(interactions);
        }

    }


}
