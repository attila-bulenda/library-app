using library_app.Models.UserDtos;
using Microsoft.AspNetCore.Identity;

namespace library_app.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(LibraryUserDto libraryUserDto);
        Task<AuthResponseDto> Login(LoginLibraryUserDto loginLibraryUserDto);
    }
}
