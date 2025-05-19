using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using UrlExtractorApi.Services;
using Azure;
using System.ClientModel;


var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Services.AddControllers();            //  启用 [ApiController]
builder.Services.AddEndpointsApiExplorer();

var innerChatClient = new AzureOpenAIClient(
    new Uri(builder.Configuration["AI:Endpoint"]!),
    new ApiKeyCredential(builder.Configuration["AI:Key"]!))  //存在userSecretsId里 dotnet user-secrets init
    .GetChatClient("gpt-4o-mini").AsIChatClient();


builder.Services.AddSingleton<IChatClient>(innerChatClient);
builder.Services.AddScoped<ArticleSummaryService>();
// builder.Services.AddAzureOpenAIChatClient("azure", options =>
// {
//     options.Endpoint = new Uri(builder.Configuration["AI:Endpoint"]!);
//     options.ApiKey = builder.Configuration["AI:Key"]!;
//     options.DeploymentName = "gpt-4o-mini";
// });

var app = builder.Build();



// 注册 controller 路由
app.MapControllers();  // 关键一行，告诉系统去扫描 Controllers/ 中的 API 类

app.UseHttpsRedirection();
app.Run();






