using Core.ObjectsSystem;

namespace GameLogic.Networks
{
    public interface IContextExecutor<in TContext>: IDroppable
    {
        bool Execute(TContext context, out string message, params string[] parameters);
    }
}