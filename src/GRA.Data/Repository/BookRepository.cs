using GRA.Domain.Model;
using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class BookRepository
        : AuditingRepository<Model.Book, Book>, IBookRepository
    {
        public BookRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BookRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddSaveForUserAsync(int requestedByUserId, int userId, Book book)
        {
            book.CreatedBy = requestedByUserId;
            book.CreatedAt = DateTime.Now;
            book = await AddSaveAsync(requestedByUserId, _mapper.Map<Model.Book>(book));

            _context.UserBooks.Add(new Model.UserBook
            {
                BookId = book.Id,
                CreatedAt = book.CreatedAt,
                CreatedBy = requestedByUserId,
                UserId = userId,
            });

            await SaveAsync();
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
            await RemoveSaveAsync(requestedByUserId, bookId);
        }
    }
}
