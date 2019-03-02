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
                //if (x.IsGenerateMode)
                {
                    if (x.CustomerId == 0)
                        x.CustomerId = rand.Next(0, 1000); 
                    Console.WriteLine($"Generating {x.InteractionNumber} interactions for {x.CustomerId}");
                    await service.GenerateInteractions(x.CustomerId, x.InteractionNumber);
                    Console.WriteLine("Generation finished");
                }
                //if (!string.IsNullOrWhiteSpace(x.ImportFile))
                //{
                //    Console.WriteLine($"Importing from file {x.ImportFile}");
                //    await service.ImportFromFileAsync(x.ImportFile);
                //    Console.WriteLine($"Import finished");
                //}
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
        [Option('g', "generate", HelpText = "Will generate interactions", Default = true)]
        public bool IsGenerateMode { get; set; }
        [Option('n', "number", HelpText = "Number of interactions to generate", Default = 10)]
        public int InteractionNumber { get; set; }
        [Option('c', "customerid", HelpText = "Customer id to generate interactions", Default = 1234)]
        public int CustomerId { get; set; }

    }
}
