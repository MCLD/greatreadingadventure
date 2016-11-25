using System;

namespace GRA.Domain.Model.Abstract
{
    public abstract class BaseDomainEntity
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
