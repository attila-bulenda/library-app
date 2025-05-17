using AutoMapper;
using library_app.Contracts;
using library_app.Models.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace library_app.Identity
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<LibraryUser> _usermanager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public AuthManager(IMapper mapper, UserManager<LibraryUser> userManager, IConfiguration configuration) 
        {
            _mapper = mapper;
            _usermanager = userManager;
            _config = configuration;
        }

        public async Task<AuthResponseDto> Login(LoginLibraryUserDto loginLibraryUserDto)
        {
            var user = await _usermanager.FindByEmailAsync(loginLibraryUserDto.Email);
            bool isValidUser = await _usermanager.CheckPasswordAsync(user, loginLibraryUserDto.Password);
            if(user == null || !isValidUser)
            {
                return null;
            }
            var token = await GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id
            };
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
        private async Task<string> GenerateToken(LibraryUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _usermanager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _usermanager.GetClaimsAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims).Union(roleClaims);
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
