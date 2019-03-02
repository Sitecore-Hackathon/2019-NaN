using Sitecore.XConnect.Client;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker
{
    public static class Constants
    {
        public static class DemoGoal
        {
            public const string ProjectionTableName = "DemoGoal";
            public const string ProjectionResultTableName = "DemoGoalResultTable";
            public const string ProjectionKey = "ID";
            public const string ProjectionEngagementValue = "EngagementValue";
            public const string ProjectionTimestamp = "Timestamp";

            public const string ProjectionContactTableName = "DemoContact";
            public const string ProjectionContactResultTableName = "DemoContactResultTable";

            public const string CustomerIdKey = "CustomerId";
        }

        public static class XConnect
        {
            public const string IdentificationSource = "demo";
            public const string IdentificationSourceEmail = "demo_email";
        }
    }
}