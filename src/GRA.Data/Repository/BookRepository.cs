using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class BookRepository
        : AuditingRepository<Model.Book, Book>, IBookRepository
    {
        public BookRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BookRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> AddSaveForUserAsync(int requestedByUserId, int userId, Book book)
        {
            book.CreatedBy = requestedByUserId;
            book.CreatedAt = _dateTimeProvider.Now;
            book = await AddSaveAsync(requestedByUserId, _mapper.Map<Model.Book>(book));

            _context.UserBooks.Add(new Model.UserBook
            {
                BookId = book.Id,
                CreatedAt = book.CreatedAt,
                CreatedBy = requestedByUserId,
                UserId = userId,
            });

            await SaveAsync();

            return book.Id;
        }

        public async Task<IEnumerable<Book>> GetForUserAsync(int userId)
        {
            return await _context.UserBooks
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.Book)
                .ProjectTo<Book>()
                .ToListAsync();
        }

        public async Task<int> GetCountForUserAsync(int userId)
        {
            return await _context.UserBooks
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .CountAsync();
        }

        public async Task RemoveForUserAsync(int requestedByUserId, int userId, int bookId)
        {
            var joinRecord = await _context.UserBooks
                .AsNoTracking()
                .Where(_ => _.UserId == userId && _.BookId == bookId)
                .SingleOrDefaultAsync();
            _context.UserBooks.Remove(joinRecord);
            await _context.SaveChangesAsync();
        }

        public async Task<DataWithCount<ICollection<Book>>> GetPaginatedListForUserAsync(
            BookFilter filter)
        {
            var userBooks = _context.UserBooks
                .AsNoTracking()
                .Where(_ => filter.UserIds.Contains(_.UserId));

            var count = await userBooks.CountAsync();

            var bookList = userBooks.Select(_ => _.Book);

            switch (filter.SortBy)
            {
                case SortBooksBy.Date:
                    if (filter.OrderDescending)
                    {
                        bookList = bookList
                            .OrderByDescending(_ => _.CreatedAt)
                            .ThenByDescending(_ => _.Title)
                            .ThenByDescending(_ => _.Author);
                    }
                    else
                    {
                        bookList = bookList
                            .OrderBy(_ => _.CreatedAt)
                            .ThenBy(_ => _.Title)
                            .ThenBy(_ => _.Author);
                    }
                    break;
                case SortBooksBy.Title:
                    if (filter.OrderDescending)
                    {
                        bookList = bookList
                            .OrderByDescending(_ => _.Title)
                            .ThenByDescending(_ => _.Author);
                    }
                    else
                    {
                        bookList = bookList
                            .OrderBy(_ => _.Title)
                            .ThenBy(_ => _.Author);
                    }
                    break;
                case SortBooksBy.Author:
                    if (filter.OrderDescending)
                    {
                        bookList = bookList
                            .OrderByDescending(_ => _.Author)
                            .ThenByDescending(_ => _.Title);
                    }
                    else
                    {
                        bookList = bookList
                            .OrderBy(_ => _.Author)
                            .ThenBy(_ => _.Title);
                    }
                    break;
            }

            var data = await bookList
                .ApplyPagination(filter)
                .ProjectTo<Book>()
                .ToListAsync();

            return new DataWithCount<ICollection<Book>>()
            {
                Data = data,
                Count = count
            };
}

public async Task<bool> UserHasBookAsync(int userId, int bookId)
{
    return await _context.UserBooks.AsNoTracking()
        .Where(_ => _.BookId == bookId && _.UserId == userId)
        .AnyAsync();
}

public async Task<int> GetUserCountForBookAsync(int bookId)
{
    return await _context.UserBooks.AsNoTracking()
        .Where(_ => _.BookId == bookId)
        .CountAsync();
}
    }
}
