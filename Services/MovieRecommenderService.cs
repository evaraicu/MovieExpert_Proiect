using Grpc.Core;
using Microsoft.ML;
using MovieRecommender_GrpcService.Models;

namespace MovieRecommender_GrpcService.Services
{
    public class MovieRecommenderService : MovieRecommender.MovieRecommenderBase
    {
        private readonly PredictionEngine<MovieInput, MoviePrediction> _predictionEngine;

        public MovieRecommenderService()
        {
            var mlContext = new MLContext();
            string mlModelPath = "MovieRecommender.mlnet";

            ITransformer mlModel = mlContext.Model.Load(mlModelPath, out _);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<MovieInput, MoviePrediction>(mlModel);
        }

        public override Task<MovieResponse> GetMovieRecommendation(MovieRequest request, ServerCallContext context)
        {
            var input = new MovieInput
            {
                userId = request.UserId,
                movieId = request.MovieId
            };

            var prediction = _predictionEngine.Predict(input);

            return Task.FromResult(new MovieResponse
            {
                Score = prediction.Score
            });
        }
    }
}