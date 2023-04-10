using System.Net;

namespace GameLogic.Networks
{
    public class HttpHandler : RequestHandler<HttpListenerContext>
    {
        private string Route { get; }
        public HttpHandler(string route, IContextExecutor<HttpListenerContext> executor) : base(executor)
        {
            Route = $"/{route}/";
        }
        
        public NetworkAnswer GetAnswerData()
        {
            return new NetworkAnswer
            {
                id = id,
                status = isSuccess ? 200 : 500,
                errorMessage = isSuccess ? null : message,
                data =  isSuccess ? message : null
            };
        }

        protected override void OnAlive()
        {
            base.OnAlive();
            executor.SetAlive();
        }

        protected override void OnDrop()
        {
            executor.Drop();
            base.OnDrop();
        }

        protected override void ParseParams(HttpListenerContext context)
        {
            id = context.Request.Headers["RequestGuid"];
            @params = context.Request.RawUrl.Replace(Route, string.Empty).Split('/');
        }
    }
}