using HtmlAgilityPack;
using Microsoft.Playwright;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Services;
using Qdrant.Client.Grpc;
using System.Text;

namespace UrlExtractorApi.Controllers;

[ApiController]
[Route("api")]
public class ExtractController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorService _vectorService;

    public ExtractController(
        ISummaryService summaryService,
        IEmbeddingService embeddingService,
        IVectorService vectorService)
    {
        _summaryService = summaryService;
        _embeddingService = embeddingService;
        _vectorService = vectorService;
    }


    [HttpPost("extract")]
    public async Task<IActionResult> Post([FromBody] UrlRequest request)
    {

        var url = request.Url;
        Console.WriteLine($"Extracting: {url}");

        // 尝试 HtmlAgilityPack 抓取
        var result = TryHtmlAgilityPack(url);

        // 如果 HAP 解析失败（返回空），则使用 Playwright 模拟浏览器加载页面
        if (string.IsNullOrWhiteSpace(result))
        {
            result = await TryPlaywright(url);
        }

        var prompt = new StringBuilder()
                    .AppendLine("You will receive an input text and your task is to summarize the article in no more than 100 words.")
                    .AppendLine("Only return the summary. Do not include any explanation.")
                    .AppendLine("# Article content:")
                    .AppendLine($"{result}")
                    .ToString();

        var summarizedRes = await _summaryService.SummarizeAsync(prompt);
        return Ok(new { url, content = summarizedRes });
    }

    //
    [HttpPost("embedding/generate")]
    public async Task<ActionResult<float[]>> GenerateEmbedding([FromBody] GenerateEmbeddingRequest request)
    {
        return Ok(await _embeddingService.GetEmbeddingAsync(request.Text));
    }

    [HttpPut("embedding/put")]
    public async Task<ActionResult<UpdateResult>> InsertEmbedding([FromBody] InsertEmbeddingRequest request)
    {
        return Ok(await _vectorService.UpsertEmbeddingAsync(request.Url, request.NoteId, request.Text, request.Vectors));
    }

    [HttpPut("embedding/search")]
    public async Task<ActionResult<List<VectorSearchResultDto>>> searchResult([FromBody] GenerateEmbeddingRequest request)
    {
        //prompt -> embedding
        var vectorResult = await _embeddingService.GetEmbeddingAsync(request.Text);

        // vectordb -> response
        

        return Ok(await _vectorService.SearchEmbeddingAsync(request.QueryEmbedding, topK: request.TopRelatives, request.queryText));
    }

    private string? TryHtmlAgilityPack(string url)
    {
        try
        {
            var web = new HtmlWeb
            {
                // 设置 User-Agent，防止部分网站屏蔽爬虫
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                            "AppleWebKit/537.36 (KHTML, like Gecko) " +
                            "Chrome/120.0.0.0 Safari/537.36"
            };
            var doc = web.Load(url);

            //TODO 编码问题
            // using var client = new HttpClient();
            //         var bytes = client.GetByteArrayAsync(url).Result;
            //         var html = System.Text.Encoding.UTF8.GetString(bytes);
            //         var doc = new HtmlDocument();
            //         doc.LoadHtml(html);


            // 提取网页标题
            var titleNode = doc.DocumentNode.SelectSingleNode("//title");
            Console.WriteLine("Title: " + titleNode?.InnerText);

            // 提取所有段落文本
            var paragraphs = doc.DocumentNode.SelectNodes("//p");
            if (paragraphs == null) return null;

            var title = titleNode?.InnerText.Trim() ?? "";
            var paragraphText = string.Join("\n", paragraphs
                .Select(p => p.InnerText.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t)));

            return title + "\n\n" + paragraphText;
        }
        catch
        {
            return null;
        }
    }

    // 使用 Playwright 模拟浏览器加载网页并提取段落内容（用于 CSR 页面）
    private async Task<string> TryPlaywright(string url)
    {
        // 启动 Playwright 浏览器（无头模式）
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        // 打开新页面并导航到目标地址，等待网络空闲（页面渲染完成）
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // 提取所有 <p> 元素的 innerText，去除空行
        var text = await page.EvalOnSelectorAllAsync<string[]>("p", "els => els.map(e => e.innerText).filter(t => t.trim().length > 0)");
        return string.Join("\n", text);
    }
}


