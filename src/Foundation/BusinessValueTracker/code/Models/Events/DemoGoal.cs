using System;
using Sitecore.XConnect;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events
{
    public class DemoGoal : Goal
    {
        public DemoGoal(Guid definitionId, DateTime timestamp) : base(definitionId, timestamp)
        {
        }

        public static Guid DemoGoalDefinitionId { get; } = new Guid("1779CC42-EF7A-4C58-BF19-FA85D30755C9");
    }
}