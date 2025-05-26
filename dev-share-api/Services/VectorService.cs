using Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace Services;

public class VectorService : IVectorService
{
    private readonly QdrantClient _client;
    private readonly string _collection = "blotz-dev";
    private readonly int _dimensions = 4;

    public VectorService(QdrantClient qdrantClient)
    {
        _client = qdrantClient;
    }

    // init if there is no collection in vector db
    public async Task InitializeAsync() => await _client.CreateCollectionAsync(_collection, new VectorParams { Size = (ulong)_dimensions, Distance = Distance.Cosine });

    public async Task IndexingAsync(string fieldName)
    {
        await _client.CreatePayloadIndexAsync(
            collectionName: _collection,
            fieldName: fieldName
        );
    }

    public async Task<UpdateResult> UpsertEmbeddingAsync(string url, string noteId, string content, float[] vector)
    {
        // Convert string ID to ulong, or use incremental numeric IDs
        var point = new PointStruct
        {
            Id = ulong.Parse(noteId),
            Vectors = vector,
            Payload = {
                ["url"] = url,
                ["content"] = content
            }
        };
        return await _client.UpsertAsync(_collection, new List<PointStruct> { point });
    }

    public async Task<List<VectorSearchResultDto>> SearchEmbeddingAsync(float[] queryVector, int topK)
    {
        var results = await _client.SearchAsync(
            _collection,
            queryVector,
            limit: (ulong)topK
        );

        return results.Select(result =>
        {
            var payload = result.Payload;
            return new VectorSearchResultDto
            {
                Url = payload.TryGetValue("url", out var urlVal) && urlVal.KindCase == Value.KindOneofCase.StringValue
                    ? urlVal.StringValue
                    : string.Empty,
                Content = payload.TryGetValue("content", out var contentVal) && contentVal.KindCase == Value.KindOneofCase.StringValue
                    ? contentVal.StringValue
                    : string.Empty
            };
        }).ToList();
    }
}