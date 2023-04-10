using System;
using System.Collections.Generic;
using Events;

namespace GameLogic.Networks
{
    [Serializable]
    public class EventData : Dictionary<string, Dictionary<string, string>>
    {
        public static Type GetEventType(string eventTypeName)
        {
            return eventTypeName switch
            {
                nameof(InputAction) => typeof(InputAction),
                nameof(InteractionResult) => typeof(InteractionResult),
                nameof(LocationEvents) => typeof(LocationEvents),
                nameof(MenuAction) => typeof(MenuAction),
                _ => throw new ArgumentOutOfRangeException(nameof(eventTypeName), eventTypeName, null)
            };
        }
    }
}