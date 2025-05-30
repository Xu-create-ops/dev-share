using Models;

namespace Services;

public class EmbeddingShareChainHandle : BaseShareChainHandle
{

    private readonly IEmbeddingService _embeddingService;

    public EmbeddingShareChainHandle(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        var vectors = await _embeddingService.GetEmbeddingAsync(context.Summary);
        context.Vectors = vectors;
        return HandlerResult.Success();
    }
}
