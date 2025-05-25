namespace dev_share_api.Models
{
    public class EmbeddingResult
    {
        public List<string> TopCategories { get; set; }
        public Dictionary<string, double> SimilarityScores { get; set; }
        public float[] Vectors {  get; set; }
    }
}
