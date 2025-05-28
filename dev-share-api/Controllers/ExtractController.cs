using HtmlAgilityPack;
using Microsoft.Playwright;
using UrlExtractorApi.Models;
using UrlExtractorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace UrlExtractorApi.Controllers;

[ApiController]
[Route("api")]
public class ExtractController : ControllerBase{
    private readonly IChatClient _chatClient;
    private readonly ArticleSummaryService _articleSummaryService;
    public ExtractController(IChatClient chatClient,ArticleSummaryService articleSummaryService){
        _chatClient = chatClient;
        _articleSummaryService = articleSummaryService;
    }



    [HttpPost("extract")]
    public async Task<IActionResult> Post([FromBody] UrlRequest request)
    {

        var url = request.url;
        Console.WriteLine($"Extracting: {url}");

        // 尝试 HtmlAgilityPack 抓取
        var result = TryHtmlAgilityPack(url);

        // 如果 HAP 解析失败（返回空），则使用 Playwright 模拟浏览器加载页面
        if (string.IsNullOrWhiteSpace(result))
        {
            result = await TryPlaywright(url);
        }

        var summary = await _articleSummaryService.SummarizeArticleAsync(result);
        return Ok(new { url, content = summary });
    }


        // return Ok(new { url, content = result });


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


