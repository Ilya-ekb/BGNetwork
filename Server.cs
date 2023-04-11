using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Game.Contexts;

namespace Game.Networks
{
    public class Server
    {
        public string ServerPath { get; }
        public int Port { get; }

        private TcpListener listener;
        private bool isRunning;
        private Task runTask;
        private readonly WebContext webContext;

        public Server(string serverPath, int port, WebContext webContext)
        {
            Port = port;
            ServerPath = $"/{serverPath}/";
            this.webContext = webContext;
        }

        ~Server()
        {
            ReleaseUnmanagedResources();
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            isRunning = true;
            Task.Run(Listen);
        }

        public void Stop()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public void SendMessage(string ip, string message)
        {
            if (webContext[ip] is { })
                webContext[ip].SendMessage(message);
        }

        public void SendAll(string message)
        {
            foreach (var connection in webContext)
                connection.SendMessage(message);
        }

        private void Listen()
        {
            while (isRunning)
            {
                var client = listener.AcceptTcpClient();
                var clientIp = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                if (webContext[clientIp] is null || !webContext[clientIp].Alive)
                {
                    webContext[clientIp] = new Connection(client);
                    webContext[clientIp].SetAlive();
                    
                }

                var interaction = new Interaction(client, ServerPath);
                webContext[clientIp].AddInteraction(interaction);
                interaction.Dropped += _ => webContext[clientIp].RemoveInteraction(interaction);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            listener.Stop();
            webContext.Clear();
            isRunning = false;
        }
    }
}