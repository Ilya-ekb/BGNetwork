using UnityEngine;

namespace GameLogic.Networks
{
    [CreateAssetMenu(menuName = "Game/NetworkSetting/" + nameof(HttpSetting), fileName = nameof(HttpSetting), order = 0)]
    public class HttpSetting : NetworkSetting
    {
        public int port = 10023;
        public string gamepadPath = "gamepad";
        public string eventsPath = "events";
    }
}