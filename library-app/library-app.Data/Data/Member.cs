﻿namespace library_app.Data.Data
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public IList<Book> BooksLoaned { get; set; }
    }
}
