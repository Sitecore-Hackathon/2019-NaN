using System;
using CommandLine;

namespace Hackathon.Boilerplate.Project.ConsoleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var rand = new Random();
            var service = new InteractionService();
            Parser.Default.ParseArguments<Options>(args).WithParsed(async x =>
            {
                service.instanceUrl = x.Host;
                DateTime start = x.StartDate;
                if (start == default(DateTime))
                {
                    start = new DateTime(2017, 12,1);
                }
                DateTime end = x.EndDate;
                if (end == default(DateTime))
                {
                    end = new DateTime(2018, 12, 9);
                }
                
                if (x.IsGenerateMode)
                {
                    if (string.IsNullOrWhiteSpace(x.CustomerId ))
                        x.CustomerId = "brimit_NaN_" + Guid.NewGuid() + "@example.com"; 
                    if (x.GenerateMostValuable)
                    {
                        Console.WriteLine($"Generating interactions for {x.CustomerId} in the range {start} - {end}");
                        await service.GenerateMostValuableCustomers(x.CustomerId, x.InteractionNumber, start, end);
                        Console.WriteLine("Generation finished");
                        return;
                    }
                    if (x.GenerateLeastValuable)
                    {
                        Console.WriteLine($"Generating interactions for {x.CustomerId} in the range {start} - {end}");
                        await service.GenerateLeastValuableCustomers(x.CustomerId, start, end);
                        Console.WriteLine("Generation finished");
                        return;
                    }
                    Console.WriteLine($"Generating {x.InteractionNumber} interactions for {x.CustomerId}");
                    await service.GenerateInteractions(x.CustomerId, x.InteractionNumber);
                    Console.WriteLine("Generation finished");
                }
                if (!string.IsNullOrWhiteSpace(x.ImportFile))
                {
                    Console.WriteLine($"Importing from file {x.ImportFile}");
                    await service.ImportFromFileAsync(x.ImportFile);
                    Console.WriteLine($"Import finished");
                }
            });
            Console.ReadKey();
        }
    }


    class Options
    {
        [Option('h', "host", HelpText = "Host url for UT", Default = "http://sitecore.tracking.collection.service/")]
        public string Host { get; set; }
        [Option('i', "import", HelpText = "Provide a csv file", Default = "interactions.csv")]
        public string ImportFile { get; set; }
        [Option('g', "generate", HelpText = "Will generate interactions")]
        public bool IsGenerateMode { get; set; }
        [Option('n', "number", HelpText = "Number of interactions to generate", Default = 1000)]
        public int InteractionNumber { get; set; }
        [Option('c', "customerid", HelpText = "Customer id to generate interactions")]
        public string CustomerId { get; set; }
        [Option('s', "startdate", HelpText ="Start date of date range")]
        public DateTime StartDate { get; set; }
        [Option('e', "enddate", HelpText ="End date of date range")]
        public DateTime EndDate { get; set; }
        [Option("max", HelpText ="Generate most valuable user. Provide customerid to generate more interactions for user")]
        public bool GenerateMostValuable { get; set; }
        [Option("min", HelpText ="Generate least valuable user. Provide customerid to generate more interactions for user")]
        public bool GenerateLeastValuable { get; set; }

    }
}
