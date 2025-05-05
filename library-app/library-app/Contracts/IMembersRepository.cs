using library_app.Data;

namespace library_app.Contracts
{
    public interface IMembersRepository: IGenericRepository<Member>
    {
        Task<Member> FindMemberWithBooksLoanedAsync(int id);
        Task LoanOutBookToMemberAsync(Book book, Member member);
        Task ReturnBookToLibraryAsync(Book book, Member member);
    }
}
