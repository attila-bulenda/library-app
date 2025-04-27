using Microsoft.EntityFrameworkCore;

namespace library_app.Data
{
    public class LibraryAppDbContext: DbContext
    {
        public LibraryAppDbContext(DbContextOptions<LibraryAppDbContext> options): base(options)
        {              
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
    }
}
