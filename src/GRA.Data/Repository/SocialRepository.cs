using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.Repository
{
    public class SocialRepository : ISocialRepository
    {
        private readonly IConfiguration _config;
        private readonly Context _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEntitySerializer _entitySerializer;
        private readonly MapsterMapper.IMapper _mapper;
        private DbSet<Model.Social> _dbSet;

        public SocialRepository(ServiceFacade.Repository repositoryFacade)
        {
            if (repositoryFacade == null)
            {
                throw new ArgumentNullException(nameof(repositoryFacade));
            }
            _context = repositoryFacade.context;
            _mapper = repositoryFacade.mapper;
            _config = repositoryFacade.config;
            _dateTimeProvider = repositoryFacade.dateTimeProvider;
            _entitySerializer = repositoryFacade.entitySerializer;
        }

        protected DbSet<Model.Social> DbSet
        {
            get
            {
                return _dbSet ??= _context.Set<Model.Social>();
            }
        }

        public Task<Social> AddSaveAsync(Social social)
        {
            if (social == null)
            {
                throw new ArgumentNullException(nameof(social));
            }

            return AddSaveInternalAsync(social);
        }

        public async Task<Social> AddSaveInternalAsync(Social social)
        {
            var dbEntity = _mapper.Map<Social, Model.Social>(social);
            var added = await DbSet.AddAsync(dbEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Model.Social, Social>(added.Entity);
        }

        public async Task<ICollection<Social>> GetByHeaderIdsAsync(IEnumerable<int> headerIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => headerIds.Contains(_.SocialHeaderId))
                .ProjectToType<Social>()
                .ToListAsync();
        }

        public async Task<Social> GetByHeaderLanguageAsync(int headerId, int languageId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SocialHeaderId == headerId && _.LanguageId == languageId)
                .ProjectToType<Social>()
                .SingleOrDefaultAsync();
        }

        public async Task RemoveSaveAsync(int headerId, int languageId)
        {
            var entity = await DbSet
                .Where(_ => _.SocialHeaderId == headerId && _.LanguageId == languageId)
                .SingleOrDefaultAsync();

            if (entity != null)
            {
                DbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public Task<Social> UpdateSaveAsync(Social social)
        {
            if (social == null)
            {
                throw new ArgumentNullException(nameof(social));
            }

            return UpdateSaveInternalAsync(social);
        }

        private async Task<Social> UpdateSaveInternalAsync(Social social)
        {
            var entity = await DbSet
                .Where(_ => _.SocialHeaderId == social.SocialHeaderId
                    && _.LanguageId == social.LanguageId)
                .SingleOrDefaultAsync();
            if (entity == null)
            {
                throw new GraException("Unable to find social record.");
            }
            entity.Description = social.Description;
            entity.ImageAlt = social.ImageAlt;
            entity.ImageHeight = social.ImageHeight;
            entity.ImageLink = social.ImageLink;
            entity.ImageWidth = social.ImageWidth;
            entity.Title = social.Title;
            entity.TwitterUsername = social.TwitterUsername;
            var updated = DbSet.Update(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Model.Social, Social>(updated.Entity);
        }
    }
}
