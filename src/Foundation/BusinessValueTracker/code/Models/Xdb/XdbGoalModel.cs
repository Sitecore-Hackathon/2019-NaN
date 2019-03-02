using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
using Sitecore.XConnect.Schema;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Xdb
{
    public static class XdbGoalModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("DemoGoal", new XdbModelVersion(1, 0));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            modelBuilder.DefineEventType<DemoGoal>(false);
            return modelBuilder.BuildModel();
        }
    }
}