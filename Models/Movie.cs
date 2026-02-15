using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieExpert_Proiect.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Display(Name = "Titlu")]
        public string Title { get; set; }

        [Display(Name = "An Lansare")]
        public int ReleaseYear { get; set; }

        [Display(Name = "Durată (min)")]
        public int RuntimeMinutes { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        [Display(Name = "Rating IMDB")]
        public decimal IMDBRating { get; set; }

        [Display(Name = "Poster")]
        public string PosterUrl { get; set; }

        [Display(Name = "Descriere")]
        public string Overview { get; set; }

        public int GenreId { get; set; }

        [Display(Name = "Gen")]
        public Genre Genre { get; set; }

        public int DirectorId { get; set; }

        [Display(Name = "Regizor")]
        public Director Director { get; set; }

        public int ActorId { get; set; }

        [Display(Name = "Actor Principal")]
        public Actor Actor { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}