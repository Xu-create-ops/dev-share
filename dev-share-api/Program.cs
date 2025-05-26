using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using dev_share_api.Services;
using Azure;
using UrlExtractorApi.Services;
using System.ClientModel;
using OpenAI.Embeddings;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Services.AddOpenApi();
builder.Services.AddControllers();            //  启用 [ApiController]
builder.Services.AddEndpointsApiExplorer();

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var innerChatClient = new AzureOpenAIClient(
    new Uri(Environment.GetEnvironmentVariable("GPT_ENDPOINT")!), 
    new ApiKeyCredential(Environment.GetEnvironmentVariable("GPT_APIKEY")!))  //存在userSecretsId里 dotnet user-secrets init
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();


builder.Services.AddSingleton<IChatClient>(innerChatClient);
builder.Services.AddScoped<ArticleSummaryService>();


// builder.Services.AddAzureOpenAIChatClient("azure", options =>
// {
//     options.Endpoint = new Uri(builder.Configuration["AI:Endpoint"]!);
//     options.ApiKey = builder.Configuration["AI:Key"]!;
//     options.DeploymentName = "gpt-4o-mini";
// });

builder.Services.AddSingleton<EmbeddingClient>(provider =>
{
    var endpoint = new Uri(Environment.GetEnvironmentVariable("EMBEDDING_ENDPOINT")!);
    var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("EMBEDDING_APIKEY")!);
    var model = "text-embedding-3-small";

    AzureOpenAIClient openAIclient = new AzureOpenAIClient(endpoint, credential);


    return openAIclient.GetEmbeddingClient(model);
});

// Register the embedding service
builder.Services.AddScoped<EmbeddingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "embedding api");
    });
}

// 注册 controller 路由
app.MapControllers();  // 关键一行，告诉系统去扫描 Controllers/ 中的 API 类
app.UseCors("AllowLocalhost3000");
app.UseHttpsRedirection();
app.Run();

