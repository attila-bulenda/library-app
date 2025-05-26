using library_app.Data.Data;

namespace library_app.Contracts
{
    public interface IBooksRepository: IGenericRepository<Book>
    {
        Task<Book> FindByISBNAsync(string isbn);
    }
}
