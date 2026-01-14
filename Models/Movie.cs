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

        // 1. Legătura cu Genul
        public int GenreId { get; set; }

        [Display(Name = "Gen")]
        public Genre Genre { get; set; }

        // 2. Legătura cu Regizorul
        public int DirectorId { get; set; }

        [Display(Name = "Regizor")]
        public Director Director { get; set; }

        // 3. Legătura cu Actorul Principal
        public int ActorId { get; set; }

        // Aici am facut modificarea ceruta:
        [Display(Name = "Actor Principal")]
        public Actor Actor { get; set; }

        // 4. Legătura cu Recenziile
        public ICollection<Review> Reviews { get; set; }
    }
}