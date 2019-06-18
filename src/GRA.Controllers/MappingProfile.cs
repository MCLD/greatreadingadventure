﻿using System.Linq;
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
            // Domain to ViewModel Mappings
            CreateMap<ViewModel.Shared.ProgramSettingsViewModel, GRA.Domain.Model.Program>().ReverseMap();
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
            CreateMap<AvatarBundle, AvatarBundleJsonModel.AvatarBundle>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.AvatarItems.Select(_ => _.Id)))
                .ReverseMap();
            CreateMap<SiteDetailViewModel, Site>().ReverseMap();
            CreateMap<SiteConfigurationViewModel, Site>().ReverseMap();
            CreateMap<SiteScheduleViewModel, Site>().ReverseMap();
            CreateMap<SiteSocialMediaViewModel, Site>().ReverseMap();

            // ViewModel to Domain Mappings
            CreateMap<GRA.Domain.Model.Program, ViewModel.Shared.ProgramSettingsViewModel>();
            CreateMap<User, SinglePageViewModel>();
            CreateMap<User, Step1ViewModel>();
            CreateMap<User, Step2ViewModel>();
            CreateMap<User, Step3ViewModel>();
            CreateMap<User, ParticipantsAddViewModel>();
            CreateMap<ChallengeTask, TaskDetailViewModel>();
            CreateMap<AvatarJsonModel.AvatarLayer, AvatarLayer>();
            CreateMap<Site, SiteDetailViewModel>();
            CreateMap<Site, SiteConfigurationViewModel>();
            CreateMap<Site, SiteScheduleViewModel>();
            CreateMap<Site, SiteSocialMediaViewModel>();
        }
    }
}
