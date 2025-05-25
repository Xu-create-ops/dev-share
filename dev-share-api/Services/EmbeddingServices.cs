using dev_share_api.Models;
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

        public string GetEmbeddingPrompt(EmbeddingRequest request)
        {
            return $"Context: This is a {request.Type} about {string.Join(", ", request.Tags)}.\n\nSummary:\n{request.Summary}";
        }

        public async Task<ActionResult<float[]>> GetEmbeddingAsync(EmbeddingRequest request)
        {
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(GetEmbeddingPrompt(request), options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            float[] result = vector.ToArray();

            return result;
        }
    }
}
