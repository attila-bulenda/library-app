using library_app.Contracts;
using library_app.Data;
using Microsoft.EntityFrameworkCore;

namespace library_app.Repository
{
    public class BooksRepository: GenericRepository<Book>, IBooksRepository
    {
        private readonly LibraryAppDbContext _context;
        public BooksRepository(LibraryAppDbContext context): base(context) 
        {        
            _context = context;
        }
        public async Task<Book> FindByISBNAsync(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }
    }
}
