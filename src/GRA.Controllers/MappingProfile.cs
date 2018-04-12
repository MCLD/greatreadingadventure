using System.Linq;
using GRA.Controllers.ViewModel.Avatar;
using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Join;
using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.MissionControl.Sites;
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
            CreateMap<ParticipantsAddViewModel, User>().ReverseMap();
            CreateMap<TaskDetailViewModel, ChallengeTask>().ReverseMap();
            CreateMap<AvatarLayer, AvatarJsonModel.AvatarLayer>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.AvatarItems.Select(_ => _.Id)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.AvatarColors
                .Select(_ => new AvatarJsonModel.AvatarColor
                {
                    Id = _.Id,
                    Value = _.Color
                })))
                .ReverseMap();
            CreateMap<SiteDetailViewModel, Site>().ReverseMap();
            CreateMap<SiteConfigurationViewModel, Site>().ReverseMap();
            CreateMap<SiteScheduleViewModel, Site>().ReverseMap();
            CreateMap<SiteSocialMediaViewModel, Site>().ReverseMap();
        }
    }
}
