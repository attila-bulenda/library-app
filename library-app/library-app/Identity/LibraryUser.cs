using Microsoft.AspNetCore.Identity;

namespace library_app.Identity
{
    public class LibraryUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
