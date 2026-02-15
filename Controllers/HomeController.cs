using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Data;
using MovieExpert_Proiect.Models;

namespace MovieExpert_Proiect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
           
            var totalMovies = await _context.Movies.CountAsync();
            var totalReviews = await _context.Reviews.CountAsync();

           
            var globalRating = totalMovies > 0
                ? (double)await _context.Movies.AverageAsync(m => m.IMDBRating)
                : 0;

            
            var genreStats = await _context.Movies
                .Include(m => m.Genre)
                .Where(m => m.Genre != null)
                .GroupBy(m => m.Genre.Name)
                .Select(g => new GenreStat
                {
                    GenreName = g.Key,
                    MovieCount = g.Count(),
                    AverageRating = (double)g.Average(m => m.IMDBRating)
                })
                .OrderByDescending(g => g.MovieCount)
                .ToListAsync();

          
            var viewModel = new DashboardViewModel
            {
                TotalMovies = totalMovies,
                TotalReviews = totalReviews,
                GlobalAverageRating = globalRating,
                GenreStats = genreStats,
                ChartLabels = genreStats.Select(g => g.GenreName).ToList(),
                ChartValues = genreStats.Select(g => g.MovieCount).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}