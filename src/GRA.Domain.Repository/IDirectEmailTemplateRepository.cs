﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailTemplateRepository : IRepository<Model.DirectEmailTemplate>
    {
        public Task<int> AddSaveWithTextAsync(int userId, DirectEmailTemplate directEmailTemplate);

        public Task<IDictionary<int, bool>> GetLanguageUnsubAsync(int directEmailTemplateId);

        public Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
                                            int languageId);

        public Task<DirectEmailTemplate> GetWithTextBySystemId(string systemEmailId,
            int languageId);

        public Task IncrementSentCountAsync(int directEmailTemplateId, int incrementBy);

        public Task<ICollectionWithCount<EmailTemplateListItem>> PageAsync(BaseFilter filter);

        public Task UpdateSaveWithText(int userId, DirectEmailTemplate directEmailTemplate);

        public Task UpdateSentBulkAsync(int directEmailTemplateId);
    }
}
