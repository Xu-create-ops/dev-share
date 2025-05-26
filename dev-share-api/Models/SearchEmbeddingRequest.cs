namespace Models;

public class SearchEmbeddingRequest
{
    public required float[] QueryEmbedding { get; set; }
    public int TopRelatives { get; set; }
}