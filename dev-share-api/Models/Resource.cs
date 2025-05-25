namespace dev_share_api.Models
{
    public class Resource
    {
        public string Title { get; set; }
        public string Type { get; set; } //Type of the resource, e.g. blogs, videos, etc.
        public List<string> Tags { get; set; }
        public string Summary { get; set; }
    }
}