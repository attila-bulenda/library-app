using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library_app.Data;
using library_app.Models.MemberDtos;
using AutoMapper;
using library_app.Contracts;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMembersRepository _membersRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public MembersController(IMapper mapper, IMembersRepository membersRepository, IBooksRepository booksRepository)
        {
            _membersRepository = membersRepository;
            _booksRepository = booksRepository;
            _mapper = mapper;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var result = await _membersRepository.GetAllAsync();    
            var members = _mapper.Map<List<MemberDto>>(result);
            return Ok(members);
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberFullDto>> GetMember(int id)
        {
            var result = await _membersRepository.FindMemberWithBooksLoanedAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var member = _mapper.Map<MemberFullDto>(result);
            return Ok(member);
        }

        // POST: api/members/loan/1/978-1-345-67890-1
        [HttpPost("loan/{id}/{isbn}")]
        public async Task<ActionResult<Member>> LoanOutBookToMember(int id, string isbn)
        {
            var member = await _membersRepository.GetAsync(id);
            var book = await _booksRepository.FindByISBNAsync(isbn);
            if (member == null)
            {
                return NotFound($"Member not found with ID {id}.");
            }
            else if (book == null)
            {
                return NotFound($"Book not found with ISBN {isbn}.");
            }
            else if (book.AvailableForLoan == false)
            {
                return BadRequest("Error: book already loaned.");
            }
            await _membersRepository.LoanOutBookToMemberAsync(book, member);
            return NoContent();
        }

        // POST: api/members/return/1/978-1-345-67890-1
        [HttpPost("return/{id}/{isbn}")]
        public async Task<ActionResult<Member>> ReturnBookToLibrary(int id, string isbn)
        {
            var member = await _membersRepository.GetAsync(id);
            var book = await _booksRepository.FindByISBNAsync(isbn);
            if (member == null)
            {
                return NotFound($"Member not found with ID {id}.");
            }
            else if (book == null)
            {
                return NotFound($"Book not found with ISBN {isbn}.");
            }
            else if (book.AvailableForLoan == true)
            {
                return BadRequest("Error: book already available.");
            }
            await _membersRepository.ReturnBookToLibraryAsync(book, member);
            return NoContent();
        }
    }
}
