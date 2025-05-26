using Microsoft.OpenApi.Models;
using OpenAI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true); // overrides

// optional - if you don't want to have 'appsettings.local.json' for debugging purpose
// Load secrets in development before building
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Config sections
var openAiConfig = builder.Configuration.GetSection("OpenAI");
var qdrantConfig = builder.Configuration.GetSection("Qdrant");

// Qdrant client
builder.Services.AddSingleton<QdrantClient>(_ =>
{
    var channel = QdrantChannel.ForAddress(
        $"{qdrantConfig["Host"]}:6334",
        new ClientConfiguration { ApiKey = qdrantConfig["ApiKey"] }
    );
    return new QdrantClient(new QdrantGrpcClient(channel));
});

// OpenAI client
builder.Services.AddSingleton<OpenAIClient>(_ =>
{
    var apiKey = openAiConfig["ApiKey"]
                 ?? throw new InvalidOperationException("OpenAI:ApiKey is missing");
    return new OpenAIClient(apiKey);
});

// Application services
builder.Services.AddScoped<IVectorService>(sp =>
{
    var qdrantClient = sp.GetRequiredService<QdrantClient>();
    return new VectorService(qdrantClient);
});

builder.Services.AddScoped<IEmbeddingService>(sp =>
{
    var openAiClient = sp.GetRequiredService<OpenAIClient>();
    return new EmbeddingService(openAiClient);
});

builder.Services.AddScoped<ISummaryService>(sp =>
{
    var openAiClient = sp.GetRequiredService<OpenAIClient>();
    return new SummaryService(openAiClient);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost3000");
app.MapControllers();

await app.RunAsync();
