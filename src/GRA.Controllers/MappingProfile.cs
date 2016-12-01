using GRA.Controllers.ViewModel.Join;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<JoinViewModel, User>().ReverseMap();
        }
    }
}
