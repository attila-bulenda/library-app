using AutoMapper;
using library_app.Contracts;
using library_app.Data.Data;
using library_app.Models.BookDtos;
using Microsoft.EntityFrameworkCore;

namespace library_app.Service
{
    public class BooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksService(IBooksRepository booksRepository, IMapper mapper)
        {
          _booksRepository = booksRepository;
          _mapper = mapper;
        }

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            var books = await _booksRepository.GetAllAsync();
            var records = _mapper.Map<List<BookDto>>(books);
            return records;
        }        
       
        public async Task UpdateBook(Book searchResult, BookDto updateBookDto)
        {
            _mapper.Map(updateBookDto, searchResult);
            try
            {
                await _booksRepository.UpdateAsync(searchResult);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        
        public async Task<Book> AddBook(CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.AvailableForLoan = true;
            await _booksRepository.AddAsync(book);
            return book;
        }    

        public BookDto MapSearchResult(Book searchResult)
        {
            if (searchResult == null)
            {
                return null;
            }
            var book = _mapper.Map<BookDto>(searchResult);
            return book;
        }
    }
}
