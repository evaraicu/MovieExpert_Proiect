using System;
using System.ComponentModel.DataAnnotations;

namespace MovieExpert_Proiect.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name = "Nume Utilizator")]
        public string UserName { get; set; }

        [Display(Name = "Comentariu")]
        public string Content { get; set; }

        [Range(1, 10)]
        [Display(Name = "Notă (1-10)")]
        public int Rating { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

        public string? Sentiment { get; set; }

        
        public int MovieId { get; set; }

        public Movie Movie { get; set; }
    }
}