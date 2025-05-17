using Microsoft.AspNetCore.Mvc;
using library_app.Models.MemberDtos;
using library_app.Service;
using Microsoft.AspNetCore.Authorization;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Librarian")]
    public class MembersController : ControllerBase
    {
        private readonly MembersService _membersService;

        public MembersController(MembersService membersService)
        {
            _membersService = membersService;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var members = await _membersService.GetAllMembersAsync();
            return Ok(members);
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberFullDto>> GetMember(int id)
        {
            var member = await _membersService.GetMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        // POST: api/members/loan/1/978-1-345-67890-1
        [HttpPost("loan/{id}/{isbn}")]
        public async Task<ActionResult> LoanOutBookToMember(int id, string isbn)
        {
            var error = await _membersService.LoanBookAsync(id, isbn);
            if (error == "Member not found") return NotFound(error);
            if (error == "Book not found") return NotFound(error);
            if (error != null) return BadRequest(error);
            return NoContent();
        }

        // POST: api/members/return/1/978-1-345-67890-1
        [HttpPost("return/{id}/{isbn}")]
        public async Task<ActionResult> ReturnBookToLibrary(int id, string isbn)
        {
            var error = await _membersService.ReturnBookAsync(id, isbn);
            if (error == "Member not found") return NotFound(error);
            if (error == "Book not found") return NotFound(error);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
