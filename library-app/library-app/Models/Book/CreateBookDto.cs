using System.ComponentModel.DataAnnotations;

namespace library_app.Models.Book
{
    public class CreateBookDto
    {
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public int PublicationYear { get; set; }
    }
}
