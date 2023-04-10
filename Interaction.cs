using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core;
using Core.Locations.Model;
using Core.ObjectsSystem;
using UnityEngine;

namespace GameLogic.Networks
{
    public class Interaction : IDroppable
    {
        public string Name { get; }
        public bool Alive { get; private set; }
        public event Action<IDroppable> Dropped;

        private const string ErrorMessage = "None";
        private const string MainPage = "index.html";
        private readonly string serverPath;
        private readonly Hashtable contents;

        private TcpClient client;
        private Thread interact;

        public Interaction(TcpClient client, string serverPath)
        {
            this.serverPath = serverPath;
            this.client = client;
            contents = new Hashtable();
            Alive = true;
            SetContents();
            interact = new Thread(Interact);
            interact.Start();
        }
        
        ~Interaction()
        {
            Drop();
        }
        
        public void Drop()
        {
            Dropped?.Invoke(this);
            Alive = false;
            client.Close();
            client.Dispose();
            client = null;
            contents.Clear();
            interact.Abort();
            interact.Join();
            interact = null;
        }

        public void SetAlive(IDroppable location = null)
        {
            
        }

        private string GetPath(string request)
        {
            var space1 = request.IndexOf(" ");
            var space2 = request.IndexOf(" ", space1 + 1);
            var url = request.Substring(space1 + 2, space2 - space1 - 2);
            if (url == "")
                url = MainPage;
            return serverPath + url;
        }

        private string GetContent(string filePath)
        {
            var ext = "";
            var dot = filePath.LastIndexOf(".");
            if (dot >= 0)
                ext = filePath.Substring(dot, filePath.Length - dot).ToUpper();
            if (contents[ext] == null)
                return "application/" + ext;
            return (string) contents[ext];
        }

        private void WriteHeaderToClient(string contentType, long length)
        {
            var str = "HTTP/1.1 200 OK\nContent-type: " + contentType + "\nContent-Encoding: 8bit\nContent-Length:" + length + "\n\n";
            client.GetStream().Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
        }

        private void WriteToClient(string responce)
        {
            string path = new DirectoryInfo(System.Environment.CurrentDirectory).FullName;
            string filePath = path + GetPath(responce);
            if (filePath.IndexOf("..") >= 0 || !File.Exists(filePath))
            {
                WriteHeaderToClient("text/plain", ErrorMessage.Length);
                client.GetStream().Write(Encoding.ASCII.GetBytes(ErrorMessage), 0, ErrorMessage.Length);
                return;
            }

            FileStream file = File.Open(filePath, FileMode.Open);
            WriteHeaderToClient(GetContent(filePath), file.Length);
            byte[] buf = new byte[1024];
            int len;
            Utilities.Log($"Write to client {client.Client.RemoteEndPoint} {responce}", Color.yellow);
            while ((len = file.Read(buf, 0, 1024)) != 0)
                client.GetStream().Write(buf, 0, len);
            file.Close();
            Drop();
        }

        private void Interact()
        {
            try
            {
                var buffer = new byte[1024];
                var request = "";
                while (true)
                {
                    var count = client.GetStream().Read(buffer, 0, 1024);
                    request += Encoding.ASCII.GetString(buffer, 0, count);
                    if (request.IndexOf("\r\n\r\n", StringComparison.Ordinal) < 0)
                        continue;
                    
                    Utilities.Log($"Received from {client.Client.RemoteEndPoint} {request}", Color.cyan);
                    
                    WriteToClient(request);
                    request = "";
                }
            }
            catch (Exception )
            {
            }
        }

        private void SetContents()
        {
            contents.Clear();
            contents.Add("", "application/unknown");
            contents.Add(".WASM", "application/wasm");
            contents.Add(".HTML", "text/html");
            contents.Add(".HTM", "text/html");
            contents.Add(".CSS", "text/css");
            contents.Add(".JS", "text/javascript");
            contents.Add(".TXT", "text/plain");
            contents.Add(".GIF", "image/gif");
            contents.Add(".SVG", "image/svg+xml");
            contents.Add(".JPG", "image/jpeg");
        }
    }
}