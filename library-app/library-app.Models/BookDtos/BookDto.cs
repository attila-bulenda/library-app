﻿namespace library_app.Models.BookDtos
{
    public class BookDto
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public bool AvailableForLoan { get; set; }
    }
}
