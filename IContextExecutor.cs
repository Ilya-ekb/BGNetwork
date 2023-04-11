using Core.ObjectsSystem;

namespace Game.Networks
{
    public interface IContextExecutor<in TContext>: IDroppable
    {
        bool Execute(TContext context, out string message, params string[] parameters);
    }
}