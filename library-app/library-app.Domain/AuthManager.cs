using AutoMapper;
using library_app.Contracts;
using library_app.Models.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private LibraryUser _user;

        private const string _loginProvider = "LibraryAppApi";
        private const string _refreshToken = "RefreshToken";
        public AuthManager(IMapper mapper, UserManager<LibraryUser> userManager, IConfiguration configuration) 
        {
            _mapper = mapper;
            _usermanager = userManager;
            _config = configuration;
        }

        public async Task<IEnumerable<IdentityError>> Register(LibraryUserDto libraryUserDto)
        {
            _user = _mapper.Map<LibraryUser>(libraryUserDto);
            _user.UserName = libraryUserDto.Email;
            var result = await _usermanager.CreateAsync(_user, libraryUserDto.Password);
            if (result.Succeeded)
            {
                await _usermanager.AddToRoleAsync(_user, "Librarian");
            }
            return result.Errors;
        }

        public async Task<AuthResponseDto> Login(LoginLibraryUserDto loginLibraryUserDto)
        {
            _user = await _usermanager.FindByEmailAsync(loginLibraryUserDto.Email);
            bool isValidUser = await _usermanager.CheckPasswordAsync(_user, loginLibraryUserDto.Password);
            if(_user == null || !isValidUser)
            {
                return null;
            }
            var token = await GenerateToken();
            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _usermanager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _usermanager.GetClaimsAsync(_user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
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

        public async Task<string> CreateRefreshToken()
        {
            await _usermanager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _usermanager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _usermanager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
        }
        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto authResponseDto)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(authResponseDto.Token);
            var userName = tokenContent.Claims.ToList()
                .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?
                .Value;
            _user = await _usermanager.FindByNameAsync(userName);
            if(_user == null || _user.Id != authResponseDto.UserId)
            {
                return null;
            }
            var isValidRefreshToken = await _usermanager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, authResponseDto.RefreshToken);
            if(isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }
            await _usermanager.UpdateSecurityStampAsync(_user);
            return null;
        }
    }
}
