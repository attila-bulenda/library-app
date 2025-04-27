using Microsoft.EntityFrameworkCore;

namespace library_app.Data
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public bool AvailableForLoan { get; set; }
    }
}
