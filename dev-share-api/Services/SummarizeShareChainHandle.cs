using Models;

namespace Services;


public class SummarizeShareChainHandle : BaseShareChainHandle
{

    private readonly ISummaryService _summaryService;

    public SummarizeShareChainHandle(ISummaryService summaryService)
    {
        _summaryService = summaryService;
    }


    protected async override Task<HandlerResult> ProcessAsync(ResourceShareContext context)
    {
        var summary = await _summaryService.SummarizeAsync(context.Prompt);
        context.Summary = summary;
        return HandlerResult.Success();
    }
}
