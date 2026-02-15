using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Data;
using MovieExpert_Proiect.Models;
using MovieTrivia_GrpcService;

namespace MovieExpert_Proiect.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TriviaService.TriviaServiceClient _triviaClient;

        public MoviesController(ApplicationDbContext context, TriviaService.TriviaServiceClient triviaClient)
        {
            _context = context;
            _triviaClient = triviaClient;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string sortOrder, string searchString, string genreFilter, decimal? minRating, int? year)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["GenreFilter"] = genreFilter;
            ViewData["MinRating"] = minRating;
            ViewData["Year"] = year;

            var movies = _context.Movies
                .Include(m => m.Actor)
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genreFilter))
                movies = movies.Where(m => m.Genre.Name == genreFilter);

        
            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString)
                                       || s.Director.Name.Contains(searchString)
                                       || s.Actor.Name.Contains(searchString));
            }
           

            if (minRating.HasValue)
                movies = movies.Where(m => m.IMDBRating >= minRating.Value);

            if (year.HasValue)
                movies = movies.Where(m => m.ReleaseYear == year.Value);

            switch (sortOrder)
            {
                case "rating_asc":
                    movies = movies.OrderBy(s => s.IMDBRating);
                    break;
                case "rating_desc":
                    movies = movies.OrderByDescending(s => s.IMDBRating);
                    break;
                case "year_asc":
                    movies = movies.OrderBy(s => s.ReleaseYear);
                    break;
                case "year_desc":
                    movies = movies.OrderByDescending(s => s.ReleaseYear);
                    break;
                default:
                    movies = movies.OrderBy(s => s.Title);
                    break;
            }

            return View(await movies.AsNoTracking().ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Actor)
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            try
            {
                var request = new TriviaRequest
                {
                    Id = movie.Id,
                    Title = movie.Title ?? "Unknown",
                    ReleaseYear = movie.ReleaseYear,
                    Genre = movie.Genre?.Name ?? "General"
                };

                var response = await _triviaClient.GetFunFactAsync(request);
                ViewBag.FunFact = response.FunFact;
                ViewBag.Emoji = response.Emoji;
            }
            catch (Exception)
            {
                ViewBag.FunFact = "Trivia momentan indisponibil.";
                ViewBag.Emoji = "⚠️";
            }

            return View(movie);
        }

      
        public async Task<IActionResult> GetRandomTrivia()
        {
            var movies = await _context.Movies.Include(m => m.Genre).ToListAsync();

            if (!movies.Any())
            {
                TempData["TriviaError"] = "Nu există filme în baza de date!";
                return RedirectToAction(nameof(Index));
            }

            var random = new Random();
            var randomMovie = movies[random.Next(movies.Count)];

            try
            {
                var request = new TriviaRequest
                {
                    Id = randomMovie.Id,
                    Title = randomMovie.Title ?? "Unknown",
                    ReleaseYear = randomMovie.ReleaseYear,
                    Genre = randomMovie.Genre?.Name ?? "General"
                };

                var response = await _triviaClient.GetFunFactAsync(request);

                TempData["TriviaFact"] = response.FunFact;
                TempData["TriviaEmoji"] = response.Emoji;
                TempData["TriviaMovie"] = randomMovie.Title;
            }
            catch (Exception)
            {
                TempData["TriviaError"] = "Nu am putut contacta expertul trivia.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name");
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            ModelState.Remove("Genre");
            ModelState.Remove("Director");
            ModelState.Remove("Actor");
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movie.ActorId);
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Name", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movie.ActorId);
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Name", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", movie.GenreId);

            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id) return NotFound();

            ModelState.Remove("Genre");
            ModelState.Remove("Director");
            ModelState.Remove("Actor");
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movie.ActorId);
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Name", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", movie.GenreId);

            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var m = await _context.Movies
                .Include(x => x.Actor)
                .Include(x => x.Director)
                .Include(x => x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (m == null) return NotFound();

            return View(m);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null) _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id) => _context.Movies.Any(e => e.Id == id);
    }
}