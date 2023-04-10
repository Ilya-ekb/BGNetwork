using UnityEngine;

namespace GameLogic.Networks
{
    [CreateAssetMenu(menuName = "Game/NetworkSetting/WebSetting", fileName = nameof(WebSetting), order = 0)]
    public class WebSetting : NetworkSetting
    {
        public string serverPath = "gamepad";
        public int port = 80;
    }
}