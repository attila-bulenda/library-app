using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library_app.Data;
using library_app.Models.MemberDtos;
using AutoMapper;
using library_app.Models.BookDtos;

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

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var result = await _context.Members.ToListAsync();
            var members = _mapper.Map<List<MemberDto>>(result);
            return Ok(members);
        }

        // GET: api/Members/5
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

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // POST: api/Members/loan/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
