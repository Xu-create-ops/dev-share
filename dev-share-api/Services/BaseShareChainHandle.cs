using Models;

namespace Services;

public abstract class BaseShareChainHandle : IShareChainHandle
{
    protected abstract Task<HandlerResult> ProcessAsync(ResourceShareContext context);

    protected void Validate(ResourceShareContext context)
    {
    }

    public Task<HandlerResult> HandleAsync(ResourceShareContext context)
    {
        try
        {
            Validate(context);
            return ProcessAsync(context); 
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(HandlerResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HandlerResult.Fail(ex.Message));
        }
    }
}