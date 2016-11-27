namespace GRA.Data.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Model.Branch, Domain.Model.Branch>().ReverseMap();
            CreateMap<Model.Challenge, Domain.Model.Challenge>().ReverseMap();
            CreateMap<Model.ChallengeTask, Domain.Model.ChallengeTask>().ReverseMap();
            CreateMap<Model.PointTranslation, Domain.Model.PointTranslation>().ReverseMap();
            CreateMap<Model.Program, Domain.Model.Program>().ReverseMap();
            CreateMap<Model.Role, Domain.Model.Role>().ReverseMap();
            CreateMap<Model.Site, Domain.Model.Site>().ReverseMap();
            CreateMap<Model.System, Domain.Model.System>().ReverseMap();
            CreateMap<Model.User, Domain.Model.User>().ReverseMap();
            CreateMap<Model.UserLog, Domain.Model.UserLog>().ReverseMap();
            CreateMap<Model.UserRole, Domain.Model.UserRole>().ReverseMap();
        }
    }
}
