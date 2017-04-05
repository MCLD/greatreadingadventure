using System.Linq;

namespace GRA.Data.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Model.Answer, Domain.Model.Answer>().ReverseMap();
            CreateMap<Model.AuthorizationCode, Domain.Model.AuthorizationCode>().ReverseMap();
            CreateMap<Model.Badge, Domain.Model.Badge>().ReverseMap();
            CreateMap<Model.Book, Domain.Model.Book>().ReverseMap();
            CreateMap<Model.Branch, Domain.Model.Branch>().ReverseMap();
            CreateMap<Model.Category, Domain.Model.Category>().ReverseMap();
            CreateMap<Model.Challenge, Domain.Model.Challenge>().ReverseMap();
            CreateMap<Model.ChallengeTask, Domain.Model.ChallengeTask>().ReverseMap();
            CreateMap<Model.Drawing, Domain.Model.Drawing>().ReverseMap();
            CreateMap<Model.DrawingCriterion, Domain.Model.DrawingCriterion>().ReverseMap();
            CreateMap<Model.PrizeWinner, Domain.Model.PrizeWinner>().ReverseMap();
            CreateMap<Model.DynamicAvatar, Domain.Model.DynamicAvatar>().ReverseMap();
            CreateMap<Model.DynamicAvatarElement, Domain.Model.DynamicAvatarElement>().ReverseMap();
            CreateMap<Model.DynamicAvatarLayer, Domain.Model.DynamicAvatarLayer>().ReverseMap();
            CreateMap<Model.EmailReminder, Domain.Model.EmailReminder>().ReverseMap();
            CreateMap<Model.EnteredSchool, Domain.Model.EnteredSchool>().ReverseMap();
            CreateMap<Model.Event, Domain.Model.Event>().ReverseMap();
            CreateMap<Model.Location, Domain.Model.Location>().ReverseMap();
            CreateMap<Model.Mail, Domain.Model.Mail>().ReverseMap();
            CreateMap<Model.Notification, Domain.Model.Notification>().ReverseMap();
            CreateMap<Model.Page, Domain.Model.Page>().ReverseMap();
            CreateMap<Model.PointTranslation, Domain.Model.PointTranslation>().ReverseMap();
            CreateMap<Model.Program, Domain.Model.Program>().ReverseMap();
            CreateMap<Model.Question, Domain.Model.Question>()
                .ForMember(dest => dest.Answers, opt =>
                {
                    opt.MapFrom(src => src.Answers.OrderBy(_ => _.SortOrder));
                    opt.ExplicitExpansion();
                })
                .ReverseMap();
            CreateMap<Model.Questionnaire, Domain.Model.Questionnaire>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.Where(_ => _.IsDeleted == false).OrderBy(_ => _.SortOrder)))
                .ReverseMap();
            CreateMap<Model.RequiredQuestionnaire, Domain.Model.RequiredQuestionnaire>().ReverseMap();
            CreateMap<Model.RecoveryToken, Domain.Model.RecoveryToken>().ReverseMap();
            CreateMap<Model.Role, Domain.Model.Role>().ReverseMap();
            CreateMap<Model.School, Domain.Model.School>().ReverseMap();
            CreateMap<Model.SchoolDistrict, Domain.Model.SchoolDistrict>().ReverseMap();
            CreateMap<Model.SchoolType, Domain.Model.SchoolType>().ReverseMap();
            CreateMap<Model.Site, Domain.Model.Site>().ReverseMap();
            CreateMap<Model.StaticAvatar, Domain.Model.StaticAvatar>().ReverseMap();
            CreateMap<Model.System, Domain.Model.System>().ReverseMap();
            CreateMap<Model.Trigger, Domain.Model.Trigger>()
                .ForMember(dest => dest.BadgeIds, opt => opt.MapFrom(src
                => src.RequiredBadges.Select(_ => _.BadgeId).ToList()))
                .ForMember(dest => dest.ChallengeIds, opt => opt.MapFrom(src
                => src.RequiredChallenges.Select(_ => _.ChallengeId).ToList()))
                .ReverseMap();
            CreateMap<Model.User, Domain.Model.User>().ReverseMap();
            CreateMap<Model.UserLog, Domain.Model.UserLog>().ReverseMap();
            CreateMap<Model.VendorCode, Domain.Model.VendorCode>().ReverseMap();
            CreateMap<Model.VendorCodeType, Domain.Model.VendorCodeType>().ReverseMap();
        }
    }
}
