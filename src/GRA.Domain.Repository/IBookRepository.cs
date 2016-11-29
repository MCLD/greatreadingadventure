using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBookRepository : IRepository<Book>
    {
        Task AddSaveForUserAsync(int requestedByUserId, int userId, Book book);
        Task<IEnumerable<Book>> GetForUserAsync(int userId);
        Task RemoveForUserAsync(int requestedByUserId, int userId, int bookId);
    }
}
