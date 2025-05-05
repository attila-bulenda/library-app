using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library_app.Data;
using library_app.Models.BookDtos;
using AutoMapper;
using library_app.Contracts;

namespace library_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBooksRepository _booksRepository;

        public BooksController(IMapper mapper, IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _booksRepository.GetAllAsync();
            var records = _mapper.Map<List<BookDto>>(books);
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
            _mapper.Map(updateBookDto, searchResult);
            try
            {
                await _booksRepository.UpdateAsync(searchResult);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return NoContent();
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.AvailableForLoan = true;
            await _booksRepository.AddAsync(book);
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
            if (searchResult == null)
            {
                return NotFound();
            }
            var book = _mapper.Map<BookDto>(searchResult);
            return Ok(book);
        }
    }
}
