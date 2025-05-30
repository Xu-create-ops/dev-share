using Models;

namespace Services;

public abstract class BaseShareChainHandle : IShareChainHandle
{
    protected abstract Task<HandlerResult> ProcessAsync(ResourceShareContext context);

    public Task<HandlerResult> HandleAsync(ResourceShareContext context)
    {
        return ProcessAsync(context); 
    }
}