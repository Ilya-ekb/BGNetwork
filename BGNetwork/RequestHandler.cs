using System;
using Core.ObjectsSystem;

namespace GameLogic.Networks
{
    public abstract class RequestHandler<TContext> : BaseDroppable
    {
        protected string id;
        protected string[] @params;
        
        protected string message;
        protected bool isSuccess = true;

        protected readonly IContextExecutor<TContext> executor;

        protected RequestHandler(IContextExecutor<TContext> executor)
        {
            this.executor = executor;
        }

        public void ProcessParams(TContext context)
        {
            isSuccess = false;
            if (!Alive)
            {
                message = "Handler is dead";
                return;
            }
            ParseParams(context);
            isSuccess = executor.Execute(context, out message, @params);
        }

        protected abstract void ParseParams(TContext context);
    }
}