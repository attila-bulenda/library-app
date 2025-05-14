using AutoMapper;
using library_app.Contracts;
using library_app.Models.UserDtos;
using Microsoft.AspNetCore.Identity;

namespace library_app.Identity
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<LibraryUser> _usermanager;
        private readonly IMapper _mapper;
        public AuthManager(IMapper mapper, UserManager<LibraryUser> userManager) 
        {
            _mapper = mapper;
            _usermanager = userManager;
        }

        public async Task<bool> Login(LoginLibraryUserDto loginLibraryUserDto)
        {
            bool isValidUser = false;
            try
            {
                var user = await _usermanager.FindByEmailAsync(loginLibraryUserDto.Email);
                isValidUser = await _usermanager.CheckPasswordAsync(user, loginLibraryUserDto.Password);            }
            catch (Exception ex)
            {
            }
            return isValidUser;
        }

        public async Task<IEnumerable<IdentityError>> Register(LibraryUserDto libraryUserDto)
        {
            var user = _mapper.Map<LibraryUser>(libraryUserDto);
            user.UserName = libraryUserDto.Email;
            var result = await _usermanager.CreateAsync(user, libraryUserDto.Password);
            if (result.Succeeded)
            {
                await _usermanager.AddToRoleAsync(user, "Librarian");
            }
            return result.Errors;
        }
    }
}
