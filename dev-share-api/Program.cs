using Azure;
using Azure.AI.OpenAI;
using dev_share_api.Services;

using OpenAI;
using OpenAI.Embeddings;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<EmbeddingClient>(provider =>
{
    var endpoint = new Uri(Environment.GetEnvironmentVariable("AZURE_ENDPOINT"));
    var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("OPENAI_APIKEY"));
    var model = "text-embedding-3-small";
    var openAIOptions = new OpenAIClientOptions()
    {
        Endpoint = endpoint
    };

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
        options.SwaggerEndpoint("/openapi/v1.json", "demo api");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();