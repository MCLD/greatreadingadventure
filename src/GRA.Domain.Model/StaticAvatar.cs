using GRA.Domain.Model.Abstract;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class StaticAvatar : BaseDomainEntity
    {
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
