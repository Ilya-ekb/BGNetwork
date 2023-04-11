using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Core;
using Core.ObjectsSystem;
using UnityEngine;

namespace Game.Networks
{
    public class Connection : IDroppable
    {
        public string Name { get; }
        public bool Alive { get; private set; }
        public TcpClient Client => client;
        public event Action<IDroppable> Dropped;

        private readonly TcpClient client;
        private readonly List<Interaction> interactions;

        public Connection(TcpClient client)
        {
            this.client = client;
            interactions = new List<Interaction>();
            Utilities.Log($"Connected {client.Client.RemoteEndPoint}", Color.green);
        }

        ~Connection()
        {
            Drop();
        }

        public void SetAlive(IDroppable location = null)
        {
            Alive = true;
        }

        public void AddInteraction(Interaction interaction)
        {
            if (!Alive)
                return;
            interactions.Add(interaction);
        }

        public void RemoveInteraction(Interaction interaction)
        {
            interactions.Remove(interaction);
        }

        public void SendMessage(string message)
        {
            var stream = client.GetStream();
            if (!stream.CanWrite)
                return;
            var data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Utilities.Log($@"Server sent {message}", Color.magenta);
        }

        public void Drop()
        {
            foreach (var interaction in interactions)
                interaction.Drop();
            interactions.Clear();
            Dropped?.Invoke(this);
        }
    }
}