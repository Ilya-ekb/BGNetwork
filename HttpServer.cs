using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Game.Contexts;
using Core;
using Core.ObjectsSystem;
using UnityEngine;

namespace Game.Networks
{
    public class HttpServer : BaseDroppable
    {
        private readonly int port;
        private readonly HttpListener httpListener;

        private readonly Dictionary<string, HttpHandler> httpHandlers;

        public HttpServer(HttpSetting setting, IContext context)
        {
            port = setting.port;
            context.AddContext(new HTTPContext(Utilities.GetLocalIPAddress(), port.ToString()));
            
            httpHandlers = new Dictionary<string, HttpHandler>
            {
                {setting.gamepadPath, new HttpHandler(setting.gamepadPath, new GamepadExecutor(context))},
                {setting.eventsPath, new HttpHandler(setting.eventsPath, new EventExecutor(context))}
            };

            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://*:{port}/");
            Debug.Log($"Initiated http://*:{port}/ ");
        }

        public void StartExecutor(string executorName)
        {
            if (httpHandlers.TryGetValue(executorName, out var handler))
                handler.SetAlive();
        }

        public void StopExecutor(string executorName)
        {
            if (httpHandlers.TryGetValue(executorName, out var handler))
                handler.Drop();
        }

        protected override void OnAlive()
        {
            base.OnAlive();
            httpListener.Start();
            httpListener.BeginGetContext(OnGetCallback, null);
            Debug.Log($"Http server started: address {Utilities.GetLocalIPAddress()}:{port}");
        }

        protected override void OnDrop()
        {
            base.OnDrop();
            httpListener.Stop();
            httpListener.Close();
            Debug.Log("Http server stopped");
        }

        private void OnGetCallback(IAsyncResult result)
        {
            var httpListenerContext = httpListener.EndGetContext(result);
            global::Core.Utilities.Log(httpListenerContext.Request.Url.ToString(), Color.cyan);
            var response = httpListenerContext.Response;
            var request = httpListenerContext.Request;
            httpListenerContext.Response.Headers.Clear();

            response.AppendHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Headers","Content-Type, Accept, X-Requested-With, RequestGuid, Content-UserName");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            response.AddHeader("Access-Control-Max-Age", "1728000");
            if (request.HttpMethod == "OPTIONS")
            {
                CreateResponse(response, new NetworkAnswer {status = 200});
                if (httpListener.IsListening)
                    httpListener.BeginGetContext(OnGetCallback, null);
                return;
            }

            try
            {
                HandleListenerContext(httpListenerContext, response);
            }
            catch (Exception e)
            {
                CreateErrorResponse(response, e.Message);
            }

            if (httpListener.IsListening)
                httpListener.BeginGetContext(OnGetCallback, null);
        }

        private static async void CreateResponse(HttpListenerResponse response, NetworkAnswer data = default)
        {
            response.SendChunked = false;
            if (data is { })
            {
                response.StatusCode = data.status;
                response.StatusDescription = data.status == 200 ? data.data : data.errorMessage;
                await using var writer = new StreamWriter(response.OutputStream, response.ContentEncoding);
                await writer.WriteAsync(JsonUtility.ToJson(data));
            }

            response.Close();
        }

        private static async void CreateErrorResponse(HttpListenerResponse response, string error)
        {
            response.SendChunked = false;
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";
            await using (var writer = new StreamWriter(response.OutputStream, response.ContentEncoding))
            {
                await writer.WriteAsync(JsonUtility.ToJson(new NetworkAnswer
                {
                    status = 500,
                    errorMessage = error,
                }));
            }

            response.Close();
        }

        private void HandleListenerContext(HttpListenerContext httpListenerContext, HttpListenerResponse response)
        {
            var url = httpListenerContext.Request.RawUrl;
            var handlerKey = url.Split('/')[1];

            if (httpHandlers.TryGetValue(handlerKey, out var handler))
            {
                handler.ProcessParams(httpListenerContext);
                CreateResponse(response, handler.GetAnswerData());
            }
            else
                CreateErrorResponse(response, $"There are no handler for url {url}");
        }
    }
}