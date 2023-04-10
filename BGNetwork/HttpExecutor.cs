using System.Net;
using Contexts;
using Core.ObjectsSystem;
using BGNetwork.Contexts;

namespace GameLogic.Networks
{
    public abstract class HttpExecutor : BaseDroppable, IContextExecutor<HttpListenerContext>
    {
        protected readonly WebContext webContext;
        protected readonly HTTPContext httpContext;

        protected HttpExecutor(IContext context)
        {
            webContext = context.GetContext<WebContext>();
            httpContext = context.GetContext<HTTPContext>();
        }
        
        public virtual bool Execute(HttpListenerContext httpListenerContext, out string message, params string[] parameters)
        {
            message = null;
            return webContext is not null && httpContext is not null;
        }
    }
}