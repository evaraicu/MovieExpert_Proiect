namespace MovieExpert_Proiect.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Un actor poate fi "Lead Actor" în mai multe filme
        public ICollection<Movie> Movies { get; set; }
    }
}