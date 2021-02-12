﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class VendorCodePackingSlipRepository
        : AuditingRepository<Model.VendorCodePackingSlip, Domain.Model.VendorCodePackingSlip>,
        IVendorCodePackingSlipRepository
    {
        public VendorCodePackingSlipRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodePackingSlipRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<VendorCodePackingSlip> GetByPackingSlipNumberAsync(long packingSlipNumber)
        {
            return await DbSet
                .Where(_ => _.PackingSlip == packingSlipNumber)
                .AsNoTracking()
                .ProjectTo<VendorCodePackingSlip>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }
    }
}
