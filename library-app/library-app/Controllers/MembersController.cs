using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library_app.Data;
using library_app.Models.MemberDtos;
using AutoMapper;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LibraryAppDbContext _context;
        private readonly IMapper _mapper;

        public MembersController(LibraryAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var result = await _context.Members.ToListAsync();
            var members = _mapper.Map<List<MemberDto>>(result);
            return Ok(members);
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberFullDto>> GetMember(int id)
        {
            var result = await _context.Members
                .Include(m => m.BooksLoaned)
                .FirstOrDefaultAsync(m => m.Id == id);

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
            var member = await _context.Members.FindAsync(id);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

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

            member.BooksLoaned ??= new List<Book>();
            book.AvailableForLoan = false;
            member.BooksLoaned.Add(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/members/return/1/978-1-345-67890-1
        [HttpPost("return/{id}/{isbn}")]
        public async Task<ActionResult<Member>> ReturnBookToLibrary(int id, string isbn)
        {
            var member = await _context.Members.FindAsync(id);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

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

            book.AvailableForLoan = true;
            member.BooksLoaned.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
