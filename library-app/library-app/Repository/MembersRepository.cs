using library_app.Contracts;
using library_app.Data;
using Microsoft.EntityFrameworkCore;

namespace library_app.Repository
{
    public class MembersRepository : GenericRepository<Member>, IMembersRepository
    {
        private readonly LibraryAppDbContext _context;
        public MembersRepository(LibraryAppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Member> FindMemberWithBooksLoanedAsync(int id)
        {
            return await _context.Members
                .Include(m => m.BooksLoaned)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task LoanOutBookToMemberAsync(Book book, Member member)
        {
            member.BooksLoaned ??= new List<Book>();
            book.AvailableForLoan = false;
            member.BooksLoaned.Add(book);
            await _context.SaveChangesAsync();
        }
        public async Task ReturnBookToLibraryAsync(Book book, Member member)
        {
            book.AvailableForLoan = true;
            member.BooksLoaned.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
