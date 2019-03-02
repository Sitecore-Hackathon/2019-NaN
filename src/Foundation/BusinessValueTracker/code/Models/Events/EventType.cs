using System.Collections.Generic;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Events
{
    public enum EventType
    {
        PageView = 0,
        AddToFavorite = 1,
        FillTheForm = 2,
        PriceDownload = 3,
        Booking = 4,
    }

    public static class EventTypeExtentions
    {
        static Dictionary<EventType, string> EventIds = new Dictionary<EventType, string>
        {
            {EventType.PageView, "" },
            {EventType.AddToFavorite, "" },
            {EventType.FillTheForm, "" },
            {EventType.PriceDownload, "4B518240-1A88-4A9D-B71A-1C21BE173060" },
            {EventType.Booking, "9016E456-95CB-42E9-AD58-997D6D77AE83" },
        };

        public static string GetEventId(this EventType type)
        {

            return EventIds[type];
        }

        
    }
}