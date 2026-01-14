using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MovieExpert_Proiect.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // 1. Asigură crearea bazei de date
                context.Database.EnsureCreated();

                if (context.Movies.Any())
                {
                    SeedReviews(context);
                    return;
                }

                // 2. Citirea din CSV
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "imdb_top_1000.csv");
                if (!File.Exists(filePath)) return;

                var lines = File.ReadAllLines(filePath).Skip(1);

                foreach (var line in lines)
                {
                    try
                    {
                        var parts = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        if (parts.Length < 15) continue;

                        string poster = parts[0].Trim('"');
                        string title = parts[1].Trim('"');
                        string yearStr = parts[2].Trim('"');
                        string runtimeStr = parts[4].Replace(" min", "").Trim('"');
                        string genreName = parts[5].Split(',')[0].Trim('"');
                        string ratingStr = parts[6].Trim('"');
                        string overview = parts[7].Trim('"');
                        string directorName = parts[9].Trim('"');
                        string actorName = parts[10].Trim('"');

                        int.TryParse(yearStr, out int year);
                        int.TryParse(runtimeStr, out int runtime);
                        decimal.TryParse(ratingStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal rating);

                        // Gestionare Gen
                        var genre = context.Genres.FirstOrDefault(g => g.Name == genreName);
                        if (genre == null) { genre = new Genre { Name = genreName }; context.Genres.Add(genre); context.SaveChanges(); }

                        // Gestionare Regizor
                        var director = context.Directors.FirstOrDefault(d => d.Name == directorName);
                        if (director == null) { director = new Director { Name = directorName }; context.Directors.Add(director); context.SaveChanges(); }

                        // Gestionare Actor
                        var actor = context.Actors.FirstOrDefault(a => a.Name == actorName);
                        if (actor == null) { actor = new Actor { Name = actorName }; context.Actors.Add(actor); context.SaveChanges(); }

                        // Creare Film
                        var movie = new Movie
                        {
                            Title = title,
                            ReleaseYear = year,
                            RuntimeMinutes = runtime,
                            IMDBRating = rating,
                            Overview = overview,
                            PosterUrl = poster,
                            GenreId = genre.Id,
                            DirectorId = director.Id,
                            ActorId = actor.Id
                        };

                        context.Movies.Add(movie);
                        context.SaveChanges();
                    }
                    catch { continue; }
                }

                SeedReviews(context);
            }
        }

        private static void SeedReviews(ApplicationDbContext context)
        {
            if (context.Reviews.Any()) return;

            var random = new Random();
            var utilizatori = new[] { "Andrei_M", "Elena.Popescu", "Radu_Cinefil", "MovieLover99", "Critic_Anonim", "Sorina_V" };

            var textePozitive = new[] {
                "O capodoperă! Mi-a plăcut enorm scenariul.",
                "Actorii joacă impecabil. Un film care merită văzut de mai multe ori.",
                "Efecte vizuale incredibile și o poveste emoționantă.",
                "Cel mai bun film din acest gen pe care l-am văzut anul acesta.",
                "Muzica și imaginea sunt de nota 10."
            };

            var texteNeutre = new[] {
                "Un film bunicel, dar parca i-a lipsit ceva la final.",
                "Poveste interesantă, însă ritmul a fost cam lent.",
                "Ok pentru o seară de duminică, dar nu m-a dat pe spate.",
                "Interpretarea este bună, dar scenariul are câteva scăpări logice."
            };

            var primeleFilme = context.Movies.Take(50).ToList();

            foreach (var film in primeleFilme)
            {
                // Generăm între 1 și 3 recenzii pentru fiecare film
                int nrRecenzii = random.Next(1, 4);

                for (int i = 0; i < nrRecenzii; i++)
                {
                    bool estePozitiv = random.Next(0, 2) == 0;
                    string text = estePozitiv ? textePozitive[random.Next(textePozitive.Length)] : texteNeutre[random.Next(texteNeutre.Length)];
                    int nota = estePozitiv ? random.Next(8, 11) : random.Next(5, 8);

                    context.Reviews.Add(new Review
                    {
                        UserName = utilizatori[random.Next(utilizatori.Length)],
                        Content = text,
                        Rating = nota,
                        ReviewDate = DateTime.Now.AddDays(-random.Next(1, 200)),
                        MovieId = film.Id
                    });
                }
            }
            context.SaveChanges();
        }
    }
}