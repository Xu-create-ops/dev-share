using Models;

namespace Services;

public class VectorShareChainHandle : BaseShareChainHandle
{

    private readonly IVectorService _vectorService;

    public VectorShareChainHandle(IVectorService vectorService)
    {
        _vectorService = vectorService;
    }

    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        await _vectorService.UpsertEmbeddingAsync(context.Url, IdGenerator.GetNextId().ToString(), context.Summary, context.Vectors);
        return HandlerResult.Success();
    }
    
}