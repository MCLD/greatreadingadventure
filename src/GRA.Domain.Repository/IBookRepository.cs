using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<int> AddSaveForUserAsync(int requestedByUserId, int userId, Book book);
        Task<int> GetCountForUserAsync(int userId);
        Task<IEnumerable<Book>> GetForUserAsync(int userId);
        Task RemoveForUserAsync(int requestedByUserId, int userId, int bookId);
        Task<DataWithCount<ICollection<Book>>> GetPaginatedListForUserAsync(int userId,
            int skip,
            int take);
    }
}
