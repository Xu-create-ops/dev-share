namespace dev_share_api.Models
{
    public class EmbeddingRequest
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public List<string> Tags { get; set; }
        public string Summary { get; set; }
    }
}