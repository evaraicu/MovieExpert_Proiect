namespace MovieExpert_Proiect.Models
{
    public class Director
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Un regizor are mai multe filme
        public ICollection<Movie> Movies { get; set; }
    }
}