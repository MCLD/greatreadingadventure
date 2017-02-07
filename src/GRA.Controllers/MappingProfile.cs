using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Join;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<ViewModel.Shared.ProgramViewModel, GRA.Domain.Model.Program>().ReverseMap();
            CreateMap<SinglePageViewModel, User>().ReverseMap();
            CreateMap<Step1ViewModel, User>().ReverseMap();
            CreateMap<Step2ViewModel, User>().ReverseMap();
            CreateMap<Step3ViewModel, User>().ReverseMap();
            CreateMap<TaskDetailViewModel, ChallengeTask>().ReverseMap();
        }
    }
}
