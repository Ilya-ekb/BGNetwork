using UnityEngine;

namespace Game.Networks
{
    [CreateAssetMenu(menuName = "Game/NetworkSetting/EventSenderSetting", fileName = nameof(EventSenderSetting), order = 0)]
    public class EventSenderSetting : NetworkSetting
    {
        public string eventsPath;
    }
}