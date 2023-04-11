using System;
using System.Net;
using Game.Contexts;
using Core;
using Events;
using Game.Inputs;
using UnityEngine;

namespace Game.Networks
{
    public class GamepadExecutor : HttpExecutor
    {
        public GamepadExecutor(IContext context) : base(context)
        {
        }

        public override bool Execute(HttpListenerContext httpListenerContext, out string message,
            params string[] @params)
        {
            if (!base.Execute(httpListenerContext, out message, @params))
                return false;

            var clientId = httpListenerContext.Request.Headers["Content-UserName"];
            var clientIp = httpListenerContext.Request.RemoteEndPoint?.ToString().Split(':')[0]; 

            if (!httpContext.IsBusyServer)
                httpContext.Borrow(clientId, clientIp);

            var isValidClient = httpContext.ClientId == clientId;

            if (isValidClient)
            {
                if (Enum.TryParse<KeyCode>(@params[1], true, out var key))
                {
                    if (string.Equals(@params[0], KeyState.Down.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        Debug.Log($"<color=YELLOW>{key} down</color>");
                        GEvent.Call(InputAction.Down, key);
                        WebInput.SetKeyDown(key);
                    }
                    else
                    {
                        Debug.Log($"<color=BLUE>{key} up</color>");
                        GEvent.Call(InputAction.Up, key);
                        WebInput.SetKeyUp(key);
                    }

                    return true;
                }

                message = $"Invalid keycode in param 1 {@params[1]}. There are no keycode {@params[0]}";
                return false;
            }

            message = $"{nameof(MenuAction)}/{nameof(MenuAction.StopGame)}";

            return true;
        }

        protected override void OnDrop()
        {
            httpContext.Release();
            base.OnDrop();
        }
    }
}