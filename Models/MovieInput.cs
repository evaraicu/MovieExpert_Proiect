namespace MovieRecommender_GrpcService.Models
{
    public class MovieInput
    {
        public float userId { get; set; }
        public float movieId { get; set; }
        public float Label { get; set; }
    }
}