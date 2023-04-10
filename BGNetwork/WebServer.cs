using Contexts;
using Core;
using Core.ObjectsSystem;
using BGNetwork.Contexts;
using UnityEngine;

namespace GameLogic.Networks
{
    public class WebServer : BaseDroppable
    {
        private readonly Server tcpServer;

        public WebServer(WebSetting setting, IContext context)
        {
            var webPort = setting.port;
            var serverPath = setting.serverPath;
            var webContext = new WebContext(Utilities.GetLocalIPAddress(), webPort.ToString());
            context.AddContext(webContext);
            tcpServer = new Server(serverPath, webPort, webContext);
            Debug.Log($"Initiated web path {serverPath} port {webPort}/ ");
        }

        public void SendMessage(string ip, string message)
        {
            tcpServer.SendMessage(ip, message);
        }

        public void SendAll(string message)
        {
            tcpServer.SendAll(message);
        }

        protected override void OnAlive()
        {
            base.OnAlive();
            tcpServer.Start();
            Debug.Log($"Web server started: address {Utilities.GetLocalIPAddress()}:{tcpServer.Port}");
        }
        
        protected override void OnDrop()
        {
            base.OnDrop();
            tcpServer.Stop();
            Debug.Log("Web server stopped");
        }
    }
}