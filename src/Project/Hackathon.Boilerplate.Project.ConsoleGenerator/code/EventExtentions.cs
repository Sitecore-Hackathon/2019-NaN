using Sitecore.UniversalTrackerClient.Entities;
using System;
using System.Collections.Generic;

namespace Hackathon.Boilerplate.Project.ConsoleGenerator
{
    public static class EventExtentions
    {

        public static string definitionId = new Guid("1779CC42-EF7A-4C58-BF19-FA85D30755C9").ToString();
        public static string itemId = Guid.Parse("110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9").ToString();

        static Dictionary<GoalType, string> EventIds = new Dictionary<GoalType, string>()
        {
            {GoalType.PageView, "" },
            {GoalType.AddToFavorite, "" },
            {GoalType.FillTheForm, "" },
            {GoalType.PriceDownload, "4B518240-1A88-4A9D-B71A-1C21BE173060" },
            {GoalType.Booking, "9016E456-95CB-42E9-AD58-997D6D77AE83" },
        };

        public static string GetEventId(this GoalType type)
        {
            return EventIds[type];
        }
        public static IUTEvent GetEvent(this Interaction interaction)
        {
            //GoalType type = (GoalType)Enum.Parse(typeof(GoalType), interaction.GoalId);

            var ev = new UTEvent(interaction.Timestamp, new Dictionary<string, string>()
                {
                    {"CustomerId" , interaction.CustomerId.ToString()}
                },
                definitionId, itemId, interaction.GoalValue, "", "", new TimeSpan(3000), Guid.NewGuid().ToString());
            return new UTGoal(ev);

        }
    }
}
