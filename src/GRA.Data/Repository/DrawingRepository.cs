using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.ServiceFacade;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DrawingRepository
        : AuditingRepository<Model.Drawing, Domain.Model.Drawing>, IDrawingRepository
    {
        public DrawingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
