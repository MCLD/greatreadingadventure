namespace GRA.Data.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Model.AuthorizationCode, Domain.Model.AuthorizationCode>().ReverseMap();
            CreateMap<Model.Badge, Domain.Model.Badge>().ReverseMap();
            CreateMap<Model.Book, Domain.Model.Book>().ReverseMap();
            CreateMap<Model.Branch, Domain.Model.Branch>().ReverseMap();
            CreateMap<Model.Challenge, Domain.Model.Challenge>().ReverseMap();
            CreateMap<Model.ChallengeTask, Domain.Model.ChallengeTask>().ReverseMap();
            CreateMap<Model.EmailReminder, Domain.Model.EmailReminder>().ReverseMap();
            CreateMap<Model.Mail, Domain.Model.Mail>().ReverseMap();
            CreateMap<Model.Notification, Domain.Model.Notification>().ReverseMap();
            CreateMap<Model.Page, Domain.Model.Page>().ReverseMap();
            CreateMap<Model.PointTranslation, Domain.Model.PointTranslation>().ReverseMap();
            CreateMap<Model.Program, Domain.Model.Program>().ReverseMap();
            CreateMap<Model.RecoveryToken, Domain.Model.RecoveryToken>().ReverseMap();
            CreateMap<Model.Role, Domain.Model.Role>().ReverseMap();
            CreateMap<Model.Site, Domain.Model.Site>().ReverseMap();
            CreateMap<Model.StaticAvatar, Domain.Model.StaticAvatar>().ReverseMap();
            CreateMap<Model.System, Domain.Model.System>().ReverseMap();
            CreateMap<Model.User, Domain.Model.User>().ReverseMap();
            CreateMap<Model.UserLog, Domain.Model.UserLog>().ReverseMap();
        }
    }
}
