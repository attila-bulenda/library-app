using library_app.Data;

namespace library_app.Contracts
{
    public interface IBooksRepository: IGenericRepository<Book>
    {
        Task<Book> FindByISBNAsync(string isbn);
    }
}
