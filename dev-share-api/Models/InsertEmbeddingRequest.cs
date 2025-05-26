namespace Models;

public class InsertEmbeddingRequest
{
    public required string Url { get; set; }
    public required string Text { get; set; }
    public required string NoteId { get; set; }
    public required float[] Vectors { get; set; }
}