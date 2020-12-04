﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IList<Question>> GetByQuestionnaireIdAsync(int questionnaireId, bool includeAnswer);
        new Task RemoveSaveAsync(int userId, int id);
    }
}
