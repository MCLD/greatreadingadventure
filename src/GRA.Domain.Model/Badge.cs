using GRA.Domain.Model.Abstract;

namespace GRA.Domain.Model
{
    public class Badge : BaseDomainEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Filename { get; set; }
    }
}
