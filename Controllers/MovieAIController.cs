using Microsoft.AspNetCore.Mvc;
using MovieExpert_Proiect.Data;
using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Models;

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
                ViewBag.Message = "Nu am găsit filme pentru acest gen în baza de date.";
                return View("Index", genres);
            }

            MovieExpert_Proiect.Models.Movie? bestMovie = null;
            float highestScore = -10000f;

            foreach (var movie in movies)
            {
                var sampleData = new MovieRecommender.ModelInput()
                {
                    Genre = movie.Genre?.Name,
                    Director = movie.Director?.Name,
                    Released_Year = movie.ReleaseYear.ToString(),
                    Runtime = movie.RuntimeMinutes.ToString()
                };

                var prediction = MovieRecommender.Predict(sampleData);

                if (prediction.Score > highestScore)
                {
                    highestScore = prediction.Score;
                    bestMovie = movie;
                }
            }
            return View("Result", bestMovie);
        }
    }
}