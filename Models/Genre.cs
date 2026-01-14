namespace MovieExpert_Proiect.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Un gen are mai multe filme
        public ICollection<Movie> Movies { get; set; }
    }
}