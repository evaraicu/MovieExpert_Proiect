using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Data;
using MovieExpert_Proiect.Models;
using MovieRecommender_GrpcService;
using MovieRecommender_GrpcService.Models;


namespace MovieExpert_Proiect.Controllers
{
    public class MovieAIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieAIController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return View(genres);
        }

        [HttpPost]
        public async Task<IActionResult> Recommend(string genre)
        {
            var movies = await _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.Director)
                .Where(m => m.Genre.Name.Contains(genre))
                .ToListAsync();

            if (movies == null || !movies.Any())
            {
                var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                ViewBag.Message = "Nu am găsit filme pentru acest gen.";
                return View("Index", genres);
            }

            MovieExpert_Proiect.Models.Movie? bestMovie = null;
            float highestScore = -10000f;

            foreach (var movie in movies)
            {
                // REZOLVARE: Nu folosim global:: deoarece creează confuzie cu gRPC.
                // Folosim direct numele clasei generat în proiectul tău principal.
                // Dacă Model Builder este în acest proiect, compilatorul îl va găsi.
                var sampleData = new MovieRecommender_GrpcService.MovieRecommender.ModelInput()
                {
                    Genre = movie.Genre?.Name,
                    Director = movie.Director?.Name,
                    Released_Year = movie.ReleaseYear.ToString(),
                    Runtime = movie.RuntimeMinutes.ToString() + " min",
                    Star1 = "Default"
                };

                // Apelăm Predict direct
                var prediction = MovieRecommender_GrpcService.Model.Predict(sampleData);

                if (prediction.Score > highestScore)
                {
                    highestScore = prediction.Score;
                    bestMovie = movie;
                }
            }

            ViewBag.PredictionScore = highestScore;
            return View("Result", bestMovie);
        }
    }
}