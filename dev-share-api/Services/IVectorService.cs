using Models;
using Qdrant.Client.Grpc;

namespace Services;

public interface IVectorService
{
    Task InitializeAsync();
    Task<UpdateResult> UpsertEmbeddingAsync(string url, string noteId, string content, float[] vector);
    Task<List<VectorSearchResultDto>> SearchEmbeddingAsync(float[] queryVector, int topK);
    Task IndexingAsync(string fieldName);
}