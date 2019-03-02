using System;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Xdb;
using Sitecore.XConnect.Schema;
using Sitecore.XConnect.Serialization;

namespace XdbModelBulder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateJsonModelForFacetModel(XdbGoalModel.Model);
            CreateJsonModelForContactFacetModel(XdbContactModel.Model);

            Console.ReadLine();
        }

        private static void CreateJsonModelForFacetModel(XdbModel model)
        {
            try
            {
                var fileName = XdbGoalModel.Model.FullName + ".json";
                var json = XdbModelWriter.Serialize(model);
                System.IO.File.WriteAllText(fileName, json);

                Console.WriteLine($"Json-model file successfully created for {model.GetType()}.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating json-model for {model.GetType()}: {ex.Message}{Environment.NewLine}");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Make sure that the console app is launched WITHIN the bin-folder of the RAI-wwwRoot folder!{Environment.NewLine}");
            }

            Console.WriteLine($"--------------------------------------------------{Environment.NewLine}");
        }

        private static void CreateJsonModelForContactFacetModel(XdbModel model)
        {
            try
            {
                var fileName = XdbContactModel.Model.FullName + ".json";
                Console.WriteLine($"Creating json-model for {model.GetType()}: '{fileName}'.{Environment.NewLine}");

                var json = XdbModelWriter.Serialize(model);

                System.IO.File.WriteAllText(fileName, json);

                Console.WriteLine($"Json-model file successfully created for {model.GetType()}.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating json-model for {model.GetType()}: {ex.Message}{Environment.NewLine}");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Make sure that the console app is launched WITHIN the bin-folder of the RAI-wwwRoot folder!{Environment.NewLine}");
            }

            Console.WriteLine($"--------------------------------------------------{Environment.NewLine}");
        }
    }
}
