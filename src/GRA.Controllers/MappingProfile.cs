using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Join;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<JoinViewModel, User>().ReverseMap();
            CreateMap<TaskDetailViewModel, ChallengeTask>().ReverseMap();
        }
    }
}
