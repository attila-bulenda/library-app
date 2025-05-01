using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library_app.Data;
using library_app.Models.BookDtos;
using AutoMapper;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryAppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(LibraryAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            var records = _mapper.Map<List<BookDto>>(books);
            return Ok(records);
        }
        
        // GET: api/Books/id/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var searchResult = await _context.Books.FindAsync(id);
            return GetBookBySearchResult(searchResult);
        }

        // GET: api/Books/isbn/978-3-445-56789-0
        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
        {
            var searchResult = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            return GetBookBySearchResult(searchResult);
        }

        // PUT: api/Books/978-0-432-98765-4
        [HttpPut("{isbn}")]
        public async Task<IActionResult> PutBook(string isbn, BookDto updateBookDto)
        {
            if (isbn != updateBookDto.ISBN)
            {
                return BadRequest();
            }
            var searchResult = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            if (searchResult == null)
            {
                return NotFound();
            }
            _mapper.Map(updateBookDto, searchResult);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.AvailableForLoan = true;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/978-0-432-98765-4
        [HttpDelete("{isbn}")]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private ActionResult<BookDto> GetBookBySearchResult(Book searchResult)
        {
            if (searchResult == null)
            {
                return NotFound();
            }
            var book = _mapper.Map<BookDto>(searchResult);
            return Ok(book);
        }
    }
}
