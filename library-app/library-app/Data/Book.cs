namespace library_app.Data
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public bool AvailableForLoan { get; set; }
    }

}
