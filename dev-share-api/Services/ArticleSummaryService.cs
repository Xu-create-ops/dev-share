using Azure;
using dev_share_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using System.Text.Json;


namespace UrlExtractorApi.Services;

public class ArticleSummaryService
{
    private readonly IChatClient _chatClient;

    public ArticleSummaryService(IChatClient chatClient) // ← 自动注入 innerChatClient
    {
        _chatClient = chatClient;
    }

    public async Task<ActionResult<string>>SummarizeArticleAsync(string article)
    {
        var prompt = $$"""
        You will receive an input text and your task is to summarize the article in no more than 100 words.

        Only return the summary. Do not include any explanation.

        # Article content:

        {{article}}
        """;

        var response = await _chatClient.GetResponseAsync(prompt);
        return response.Text;
    }

    public async Task<ActionResult<EmbeddingRequest>> GetCategorisedSummaryAsync(string article)
    {
        var prompt = $$"""
        You are a helpful assistant skilled in understanding and summarising technical content. Given a tech article, please provide the following in JSON format based on this structure:

        {
          "Title": "", // A concise and descriptive title for the article
          "Type": "", // Type of content, e.g., "Tutorial", "Blogs", "Video", "Guide", etc.
          "Tags": [], // A list of 3-7 relevant technical tags, e.g., ["C#", ".NET", "Cloud", "TypeScript"]
          "Summary": "" // A brief, clear summary of the article within 100 words
        }
        Analyse the article and fill in each field accordingly. Do not invent details not present in the article. Focus on clarity, relevance, and accurate representation of the content.
        # Article content:

        {{article}}
        """;

        var response = await _chatClient.GetResponseAsync(prompt);

        return JsonSerializer.Deserialize<EmbeddingRequest>(response.Text);
    }
}