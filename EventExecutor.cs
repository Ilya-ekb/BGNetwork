using System.Net;
using Contexts;
using Core;
using Events;
using Game;
using Unity.Plastic.Newtonsoft.Json;


namespace GameLogic.Networks
{
    public class EventExecutor : HttpExecutor
    {
        public EventExecutor(IContext context) : base(context)
        {
        }

        public override bool Execute(HttpListenerContext httpListenerContext, out string message, params string[] @params)
        {
            if (!base.Execute(httpListenerContext, out message, @params))
                return false;

            var clientId = httpListenerContext.Request.Headers["Content-UserName"];
            var clientIp = httpListenerContext.Request.RemoteEndPoint?.ToString().Split(':')[0]; 
            
            if (!httpContext.IsBusyServer && !@params[0].Equals(nameof(InputAction)))
            {
                httpContext.Borrow(clientId, clientIp);
            }

            var isValidClient = httpContext.ClientId == clientId;

            if (@params[0].Equals(nameof(InputAction)) && 
                @params[1].Equals(nameof(InputAction.GameCut)))
            {
                
                message =  $"{@params[0]}/{@params[1]}/{JsonConvert.SerializeObject(Container.GameData.Get())}" ;
                return true;
            }

            var isSuccess = true;
            if (isValidClient)
            {
                isSuccess = GEvent.CallInMainThread(Container.EventData[@params[0]][@params[1]]);

                message = !isSuccess
                    ? $"<COLOR=RED>Is not called event with {@params[1]} code</COLOR>"
                    : $"{@params[0]}/{@params[1]}";
            }
            else
            {
                message = $"{nameof(MenuAction)}/{nameof(MenuAction.StopGame)}";
            }

            return isSuccess;
        }
    }
}