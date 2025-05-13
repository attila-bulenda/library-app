using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library_app.Identity
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "288407f6-be60-4483-87f0-31eaa2265f96",
                    Name = "Librarian",
                    NormalizedName = "LIBRARIAN"
                }
            );
        }
    }
}
