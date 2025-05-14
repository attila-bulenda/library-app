using System.ComponentModel.DataAnnotations;

namespace library_app.Models.UserDtos
{
    public class LibraryUserDto: LoginLibraryUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
