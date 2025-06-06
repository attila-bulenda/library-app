﻿namespace library_app.Data.Data
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public bool AvailableForLoan { get; set; }

        public int? MemberId { get; set; } 
        public Member Member { get; set; }
    }
}
