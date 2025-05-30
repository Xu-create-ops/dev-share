namespace Models;

public class ResourceShareContext
{
    public string? Url { get; set; }
    public string? Comment { get; set; }
    public string? Summary { get; set; }
    public float[]? Vectors { get; set; }
    public string? Prompt { get; set; }
}