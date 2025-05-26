using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private const string embeddingModelId = "text-embedding-3-small";

    public EmbeddingService(OpenAIClient openAIClient)
    {
        _client = openAIClient;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        //https://api-inference.huggingface.co/pipeline/feature-extraction/
        //https://router.huggingface.co/hf-inference/models/intfloat/e5-small-v2/pipeline/sentence-similarity
        // var resp = await _client.PostAsJsonAsync($"/pipeline/feature-extraction/{Model}", new { inputs = text });
        // resp.EnsureSuccessStatusCode();
        //var raw = await resp.Content.ReadFromJsonAsync<float[][]>();
        EmbeddingGenerationOptions options = new() { Dimensions = 4 };
        OpenAIEmbedding resp = await _client.GetEmbeddingClient(model: embeddingModelId).GenerateEmbeddingAsync(text, options);
        var vector = resp.ToFloats();

        return vector.ToArray();
    }
}
