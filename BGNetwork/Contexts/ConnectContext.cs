using System.Collections;
using System.Collections.Generic;
using GameLogic.Networks;
using Newtonsoft.Json.Serialization;
using UnityEngine.Networking;

namespace BGNetwork.Contexts
{
        public class HTTPContext : ConnectContext
    {
        public bool IsBusyServer { get; private set; }
        public string ClientId { get; private set; }
        public string ClientIp { get; private set; }

        public HTTPContext(string ipAddress, string port) : base(ipAddress, port)
        {
            IsBusyServer = false;
            GEvent.Attach(MenuAction.StopGame, _ => Release());
        }

        public void Borrow(string clientId, string clientIp)
        {
            ClientIp = clientIp;
            ClientId = clientId;
            IsBusyServer = true;
        }

        public void Release()
        {
            ClientId = null;
            IsBusyServer = false;
        }
    }

    public class WebContext : ConnectContext, IEnumerable<Connection>
    {
        public Connection this[string ip]
        {
            get => clients.TryGetValue(ip, out var client) ? client : null;
            set
            {
                if (clients.ContainsKey(ip))
                    clients[ip] = value;
                else
                    clients.Add(ip, value);
            }
        }

        private readonly Dictionary<string, Connection> clients;

        public WebContext(string ipAddress, string port) : base(ipAddress, port)
        {
            clients = new Dictionary<string, Connection>();
        }

        public void Clear()
        {
            clients.Clear();
        }

        public IEnumerator<Connection> GetEnumerator()
        {
            return clients.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class ConnectContext : IContext
    {
        public string Address { get; }

        protected ConnectContext(string ipAddress, string port)
        {
            Address = ipAddress + ":" + port;
        }

        public TType GetContext<TType>(Func<TType, bool> predicate = null) where TType : class, IContext
        {
            return this as TType;
        }

        public void AddContext<TType>(TType context) where TType : IContext
        {
        }

        public void RemoveContext<TType>() where TType : IContext
        {
        }
    }

    public struct RequestContext : IContext
    {
        public string request;

        public TType GetContext<TType>(Func<TType, bool> predicate = null) where TType : class, IContext
        {
            return this as TType;
        }

        public void AddContext<TType>(TType context) where TType : IContext
        {
        }

        public void RemoveContext<TType>() where TType : IContext
        {
        }
    }

    public struct ResponseContext : IContext
    {
        public UnityWebRequest.Result result;
        public byte[] data;

        public TType GetContext<TType>(Func<TType, bool> predicate = null) where TType : class, IContext
        {
            return this as TType;
        }

        public void AddContext<TType>(TType context) where TType : IContext
        {
        }

        public void RemoveContext<TType>() where TType : IContext
        {
        }
    }

}