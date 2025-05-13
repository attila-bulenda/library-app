using library_app.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace library_app.Data
{
    public class LibraryAppDbContext: IdentityDbContext<LibraryUser>
    {
        public LibraryAppDbContext(DbContextOptions<LibraryAppDbContext> options): base(options)
        {              
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
