using System.Collections.Generic;

namespace MovieExpert_Proiect.Models
{
    
    public class GenreStat
    {
        public string GenreName { get; set; } = string.Empty;
        public int MovieCount { get; set; }
        public double AverageRating { get; set; }
    }

    
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalReviews { get; set; }
        public double GlobalAverageRating { get; set; }

       
        public List<GenreStat> GenreStats { get; set; } = new List<GenreStat>();

        
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartValues { get; set; } = new List<int>();
    }
}