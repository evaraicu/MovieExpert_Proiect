using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Models; 
namespace MovieExpert_Proiect.Data
{
  
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; } = default!;
        public DbSet<Director> Directors { get; set; } = default!;
        public DbSet<Actor> Actors { get; set; } = default!;
        public DbSet<Genre> Genres { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
    }
}