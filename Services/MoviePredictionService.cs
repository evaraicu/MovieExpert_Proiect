using System.Text;
using System.Text.Json;
using MovieExpert_Proiect.Models;

namespace MovieExpert_Proiect.Services
{
    public class MoviePredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:51616/predict"; 

        public MoviePredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float> PredictScoreAsync(MovieInput input)
        {
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MoviePrediction>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Score ?? 0f;
            }
            return 0f;
        }
    }
}