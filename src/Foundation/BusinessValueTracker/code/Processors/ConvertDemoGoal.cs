using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.XConnect.DataAccess.Pipelines.ConvertToXConnectEventPipeline;
using Sitecore.XConnect;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Processors
{
    public class ConvertDemoGoal : ConvertPageEventDataToEventBase
    {
        protected override Event CreateEvent(PageEventData pageEventData)
        {
           return new DemoGoal(DemoGoal.DemoGoalDefinitionId, pageEventData.DateTime);
        }

        protected override bool CanProcessPageEventData(PageEventData pageEventData)
        {
            return pageEventData.PageEventDefinitionId == DemoGoal.DemoGoalDefinitionId;
        }
    }
}