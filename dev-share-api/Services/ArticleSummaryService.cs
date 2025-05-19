using Microsoft.Extensions.AI;


namespace UrlExtractorApi.Services;

public class ArticleSummaryService
{
    private readonly IChatClient _chatClient;

    public ArticleSummaryService(IChatClient chatClient) // ← 自动注入 innerChatClient
    {
        _chatClient = chatClient;
    }

    public async Task SummarizeArticleAsync(string article)
    {
        var prompt = $$"""
        You will receive an input text and your task is to summarize the article in no more than 100 words.

        Only return the summary. Do not include any explanation.

        # Article content:

        {{article}}
        """;

        var response = await _chatClient.GetResponseAsync(prompt);
        Console.WriteLine(response.Text);

    }
}