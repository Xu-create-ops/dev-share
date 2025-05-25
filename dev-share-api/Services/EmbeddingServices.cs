using Microsoft.AspNetCore.Mvc;
using OpenAI.Embeddings;
using System;


namespace dev_share_api.Services
{

    public class EmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly EmbeddingGenerationOptions options = new() { Dimensions = 512 };
        public EmbeddingService(EmbeddingClient embeddingClient)
        {

            _embeddingClient = embeddingClient;
        }

        public async Task<ActionResult<float[]>> GetEmbeddingAsync(string input)
        {
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(input, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            float[] result = vector.ToArray();

            return result;
        }
    }
}
