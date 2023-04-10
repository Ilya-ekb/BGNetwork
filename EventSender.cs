using System.Runtime.InteropServices;
using System.Text;
using Contexts;
using Core;
using Core.ObjectsSystem;
using Game;
using GameLogic.GameData.Contexts;
using Models.Contexts;
using Networks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic.Networks
{
    public class EventSender : BaseDroppable
    {
        private readonly IContext context;
        private readonly string hostName;
        
        public EventSender(EventSenderSetting setting, IContext context)
        {
            this.context = context;
            var ipAddress =
#if UNITY_WEBGL && !UNITY_EDITOR
                GetURLFromPage().Remove(GetURLFromPage().Length - 1);
#else
                Utilities.GetLocalIPAddress();
#endif
            context.GetContext<MainContext>().AddContext(new HTTPContext(ipAddress, "10023"));
            hostName = $"{context.GetContext<MainContext>().GetContext<HTTPContext>().Address}/{setting.eventsPath}/";
        }

        public void Send(string actionName, object data = null)
        {
            var request = new RequestContext
            {
                request = hostName + actionName,
            };

            WebRequester.SendRequest(request, OnResponseReceived, data);
        }

        private void OnResponseReceived(ResponseContext responseContext, object data)
        {
            if (responseContext.result is not UnityWebRequest.Result.Success)
                return;
            
            var answer = RestoreData(responseContext);

            var eventName = answer?.data?.Split('/');
            if (eventName is null)
                return;

            GEvent.CallInMainThread(Container.EventData[eventName[0]][eventName[1]], data);
            Debug.Log($"Event code {eventName[0]} {eventName[1]}");
        }
        
        public static NetworkAnswer RestoreData(ResponseContext response)
        {
            var jsonString = Encoding.UTF8.GetString(response.data);
            var data = JsonConvert.DeserializeObject<NetworkAnswer>(jsonString);
            if (data is not null) return data;
            Debug.Log($"Not restored {nameof(NetworkAnswer)}");
            return null;
        }

        [DllImport("__Internal")]
        private static extern string GetURLFromPage();
    }
}