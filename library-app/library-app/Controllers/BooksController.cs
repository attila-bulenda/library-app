using Microsoft.AspNetCore.Mvc;
using library_app.Data;
using library_app.Models.BookDtos;
using library_app.Contracts;
using library_app.Service;
using Microsoft.AspNetCore.Authorization;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*
     * Turning off Authorization requirements as the front-end is a simple HTML page and
     * is not able to store JWTs properly
     * [Authorize(Roles = "Librarian")]
    */
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;
        private readonly BooksService _booksService;

        public BooksController(IBooksRepository booksRepository, BooksService booksService)
        {
            _booksRepository = booksRepository;
            _booksService = booksService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var records = await _booksService.GetAllBooksAsync();
            return Ok(records);
        }
        
        // GET: api/books/id/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var searchResult = await _booksRepository.GetAsync(id);
            return GetBookBySearchResult(searchResult);
        }

        // GET: api/books/isbn/978-3-445-56789-0
        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
        {
            var searchResult = await _booksRepository.FindByISBNAsync(isbn);
            return GetBookBySearchResult(searchResult);
        }

        // PUT: api/books/978-0-432-98765-4
        [HttpPut("{isbn}")]
        public async Task<IActionResult> PutBook(string isbn, BookDto updateBookDto)
        {
            if (isbn != updateBookDto.ISBN)
            {
                return BadRequest();
            }
            var searchResult = await _booksRepository.FindByISBNAsync(isbn);
            if (searchResult == null)
            {
                return NotFound();
            }
            await _booksService.UpdateBook(searchResult, updateBookDto);
            return NoContent();
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto bookDto)
        {
            var book = await _booksService.AddBook(bookDto);
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/books/978-0-432-98765-4
        [HttpDelete("{isbn}")]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            var book = await _booksRepository.FindByISBNAsync(isbn);
            if (book == null)
            {
                return NotFound();
            }
            await _booksRepository.DeleteAsync(book.Id);
            return NoContent();
        }

        private ActionResult<BookDto> GetBookBySearchResult(Book searchResult)
        {
            var book = _booksService.MapSearchResult(searchResult);
            return book == null ? NotFound() : Ok(book);
        }
    }
}
