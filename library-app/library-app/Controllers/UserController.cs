using library_app.Contracts;
using library_app.Models.UserDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        public UserController(IAuthManager authManager)
        {
            _authManager = authManager; 
        }

        // api/user/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] LibraryUserDto libraryUserDto)
        {
            var errors = await _authManager.Register(libraryUserDto);
            if(errors.Any())
            {
                foreach(var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // api/user/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginLibraryUserDto loginLibraryUserDto)
        {
            var isValidUser = await _authManager.Login(loginLibraryUserDto);
            if (!isValidUser)
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}
