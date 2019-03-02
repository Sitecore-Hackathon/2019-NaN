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

        public string instanceUrl { get; set; }

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
            string IdentificationSource = "demo";
            string channelId = Guid.Parse("DF9900DE-61DD-47BF-9628-058E78EF05C6").ToString();

            foreach (var interaction in interactions.GroupBy(x => x.CustomerId))
            {
                var events = interaction.Select(x => x.GetEvent()).ToArray();
                var defaultInteractionQuery = UTEntitiesBuilder.Interaction()
                                                           .ChannelId(channelId)
                                                           .Initiator(InteractionInitiator.Brand)
                                                           .Contact(IdentificationSource, interaction.Key.ToString());

                var defaultInteraction = defaultInteractionQuery.Build();
                using (var session = SitecoreUTSessionBuilder.SessionWithHost(instanceUrl)
                                                        .DefaultInteraction(defaultInteraction)
                                                        .BuildSession())
                {
                    foreach (var e in events)
                    {
                        var eventRequest = UTRequestBuilder.GoalEvent(e.DefinitionId, e.Timestamp.GetValueOrDefault())
                                .EngagementValue(e.EngagementValue.GetValueOrDefault())
                                .AddCustomValues(e.CustomValues)
                                .Duration(new TimeSpan(3000))
                                .ItemId(e.ItemId)
                                .Text(e.Text).Build();
                        var eventResponse = await session.TrackGoalAsync(eventRequest);
                        Console.WriteLine("Track EVENT RESULT: " + eventResponse.StatusCode);
                    }
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

        internal async Task GenerateMostValuableCustomers(int customerId, int count, DateTime startDate, DateTime endDate)
        {
            var interactions = new List<Interaction>();
            var rand = new Random();
            var current = startDate;
            while (current <= endDate)
            {
                for (int i = 0; i < count; i++)
                {
                    var intr = new Interaction()
                    {
                        CustomerId = customerId,
                        GoalValue = rand.Next(0, 20),
                        GoalId = ((GoalType)rand.Next(0, Enum.GetNames(typeof(GoalType)).Length)).ToString(),
                        Timestamp = current.AddHours(rand.Next(-5, 5)),
                    };
                    interactions.Add(intr);
                }
                current = current.AddMonths(1);
            }
            await PushInteractiosToUT(interactions);
        }

        internal async Task GenerateLeastValuableCustomers(int customerId, DateTime startDate, DateTime endDate)
        {
            var interactions = new List<Interaction>();
            var rand = new Random();

            var halfOfRange = ((endDate - startDate) / 2);
            var current = startDate.Add(halfOfRange);

            var intr = new Interaction()
            {
                CustomerId = customerId,
                GoalValue = rand.Next(0, 20),
                GoalId = ((GoalType)rand.Next(0, Enum.GetNames(typeof(GoalType)).Length)).ToString(),
                Timestamp = current.AddHours(rand.Next((int)-halfOfRange.TotalHours, (int)halfOfRange.TotalHours)),
            };
            interactions.Add(intr);
            await PushInteractiosToUT(interactions);
        }

    }


}
