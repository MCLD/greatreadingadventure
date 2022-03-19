using System.Linq;

namespace GRA.Data.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Data to Domain Mappings
            CreateMap<Model.Answer, Domain.Model.Answer>();
            CreateMap<Model.AuthorizationCode, Domain.Model.AuthorizationCode>();
            CreateMap<Model.AvatarBundle, Domain.Model.AvatarBundle>()
                .ForMember(dest => dest.AvatarItems,
                    opt => opt.MapFrom(src => src.AvatarBundleItems.Select(_ => _.AvatarItem)));
            CreateMap<Model.AvatarColor, Domain.Model.AvatarColor>();
            CreateMap<Model.AvatarElement, Domain.Model.AvatarElement>();
            CreateMap<Model.AvatarItem, Domain.Model.AvatarItem>();
            CreateMap<Model.AvatarLayer, Domain.Model.AvatarLayer>()
                .ForMember(dest => dest.AvatarColors, opt =>
                {
                    opt.MapFrom(src => src.AvatarColors.OrderBy(_ => _.SortOrder));
                    opt.ExplicitExpansion();
                })
                .ForMember(dest => dest.AvatarItems, opt =>
                {
                    opt.MapFrom(src => src.AvatarItems.OrderBy(_ => _.SortOrder));
                    opt.ExplicitExpansion();
                });
            CreateMap<Model.Badge, Domain.Model.Badge>();
            CreateMap<Model.Book, Domain.Model.Book>();
            CreateMap<Model.Branch, Domain.Model.Branch>();
            CreateMap<Model.Broadcast, Domain.Model.Broadcast>();
            CreateMap<Model.Carousel, Domain.Model.Carousel>();
            CreateMap<Model.CarouselItem, Domain.Model.CarouselItem>();
            CreateMap<Model.Category, Domain.Model.Category>();
            CreateMap<Model.Challenge, Domain.Model.Challenge>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(
                    src => src.ChallengeCategories.Select(_ => _.Category)));
            CreateMap<Model.ChallengeGroup, Domain.Model.ChallengeGroup>()
                .ForMember(dest => dest.ChallengeIds, opt => opt.MapFrom(
                    src => src.ChallengeGroupChallenges.Select(_ => _.ChallengeId).ToList()));
            CreateMap<Model.ChallengeTask, Domain.Model.ChallengeTask>();
            CreateMap<Model.DailyLiteracyTip, Domain.Model.DailyLiteracyTip>();
            CreateMap<Model.DailyLiteracyTipImage, Domain.Model.DailyLiteracyTipImage>();
            CreateMap<Model.DashboardContent, Domain.Model.DashboardContent>();
            CreateMap<Model.DirectEmailHistory, Domain.Model.DirectEmailHistory>();
            CreateMap<Model.DirectEmailTemplate, Domain.Model.DirectEmailTemplate>();
            CreateMap<Model.DirectEmailTemplateText, Domain.Model.DirectEmailTemplateText>();
            CreateMap<Model.Drawing, Domain.Model.Drawing>();
            CreateMap<Model.DrawingCriterion, Domain.Model.DrawingCriterion>()
                .ForMember(dest => dest.ProgramIds, opt => opt.MapFrom(src
                => src.CriterionPrograms.Select(_ => _.ProgramId).ToList()));
            CreateMap<Model.EmailBase, Domain.Model.EmailBase>();
            CreateMap<Model.EmailBaseText, Domain.Model.EmailBaseText>();
            CreateMap<Model.EmailReminder, Domain.Model.EmailReminder>();
            CreateMap<Model.EmailSubscriptionAuditLog, Domain.Model.EmailSubscriptionAuditLog>()
                .ForMember(dest => dest.CreatedByName, opt => opt
                    .MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));
            CreateMap<Model.Event, Domain.Model.Event>()
                .ForMember(dest => dest.Challenge, opt => opt.Ignore())
                .ForMember(dest => dest.ChallengeGroup, opt => opt.Ignore());
            CreateMap<Model.GroupInfo, Domain.Model.GroupInfo>();
            CreateMap<Model.GroupType, Domain.Model.GroupType>();
            CreateMap<Model.Job, Domain.Model.Job>();
            CreateMap<Model.Language, Domain.Model.Language>();
            CreateMap<Model.Location, Domain.Model.Location>();
            CreateMap<Model.Mail, Domain.Model.Mail>();
            CreateMap<Model.NewsCategory, Domain.Model.NewsCategory>()
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts.Count));
            CreateMap<Model.NewsPost, Domain.Model.NewsPost>();
            CreateMap<Model.Notification, Domain.Model.Notification>();
            CreateMap<Model.PageHeader, Domain.Model.PageHeader>()
                .ForMember(dest => dest.PageLanguages,
                    opt => opt.MapFrom(src => src.Pages.Select(_ => _.Language.Name)));
            CreateMap<Model.Page, Domain.Model.Page>();
            CreateMap<Model.PointTranslation, Domain.Model.PointTranslation>();
            CreateMap<Model.PrizeWinner, Domain.Model.PrizeWinner>()
                .ForMember(dest => dest.PrizeName,
                    opt => opt.MapFrom(src => src.Drawing.Name ?? src.Trigger.AwardPrizeName));
            CreateMap<Model.Program, Domain.Model.Program>();
            CreateMap<Model.PsAgeGroup, Domain.Model.PsAgeGroup>();
            CreateMap<Model.PsBlackoutDate, Domain.Model.PsBlackoutDate>();
            CreateMap<Model.PsBranchSelection, Domain.Model.PsBranchSelection>()
                .ForMember(dest => dest.StartsAt, opt => opt.MapFrom(src =>
                    src.ScheduleStartTime.ToShortTimeString()))
                .ForMember(dest => dest.EndsAt, opt => opt.MapFrom(src =>
                    src.ScheduleStartTime.AddMinutes(src.ScheduleDuration).ToShortTimeString()));
            CreateMap<Model.PsSettings, Domain.Model.PsSettings>();
            CreateMap<Model.PsKit, Domain.Model.PsKit>();
            CreateMap<Model.PsKitImage, Domain.Model.PsKitImage>();
            CreateMap<Model.PsPerformer, Domain.Model.PsPerformer>();
            CreateMap<Model.PsPerformerImage, Domain.Model.PsPerformerImage>();
            CreateMap<Model.PsPerformerSchedule, Domain.Model.PsPerformerSchedule>();
            CreateMap<Model.PsProgram, Domain.Model.PsProgram>();
            CreateMap<Model.PsProgramImage, Domain.Model.PsProgramImage>();
            CreateMap<Model.Question, Domain.Model.Question>()
                .ForMember(dest => dest.Answers, opt =>
                {
                    opt.MapFrom(src => src.Answers.OrderBy(_ => _.SortOrder));
                    opt.ExplicitExpansion();
                });
            CreateMap<Model.Questionnaire, Domain.Model.Questionnaire>()
                .ForMember(dest => dest.Questions,
                    opt => opt.MapFrom(src => src.Questions
                        .Where(_ => !_.IsDeleted)
                        .OrderBy(_ => _.SortOrder)));
            CreateMap<Model.RecoveryToken, Domain.Model.RecoveryToken>();
            CreateMap<Model.ReportCriterion, Domain.Model.ReportCriterion>();
            CreateMap<Model.ReportRequest, Domain.Model.ReportRequest>();
            CreateMap<Model.RequiredQuestionnaire, Domain.Model.RequiredQuestionnaire>();
            CreateMap<Model.Role, Domain.Model.Role>();
            CreateMap<Model.School, Domain.Model.School>();
            CreateMap<Model.SchoolDistrict, Domain.Model.SchoolDistrict>();
            CreateMap<Model.Site, Domain.Model.Site>();
            CreateMap<Model.SiteSetting, Domain.Model.SiteSetting>();
            CreateMap<Model.Social, Domain.Model.Social>();
            CreateMap<Model.SocialHeader, Domain.Model.SocialHeader>();
            CreateMap<Model.SpatialDistanceDetail, Domain.Model.SpatialDistanceDetail>();
            CreateMap<Model.SpatialDistanceHeader, Domain.Model.SpatialDistanceHeader>();
            CreateMap<Model.System, Domain.Model.System>()
                .ForMember(_ => _.Branches,
                    opt => opt.MapFrom(src => src.Branches.OrderBy(_ => _.Name)));
            CreateMap<Model.Trigger, Domain.Model.Trigger>()
                .ForMember(dest => dest.BadgeIds, opt => opt.MapFrom(src
                => src.RequiredBadges.Select(_ => _.BadgeId).ToList()))
                .ForMember(dest => dest.ChallengeIds, opt => opt.MapFrom(src
                => src.RequiredChallenges.Select(_ => _.ChallengeId).ToList()));
            CreateMap<Model.User, Domain.Model.User>();
            CreateMap<Model.UserLog, Domain.Model.UserLog>();
            CreateMap<Model.VendorCode, Domain.Model.VendorCode>();
            CreateMap<Model.VendorCodePackingSlip, Domain.Model.VendorCodePackingSlip>();
            CreateMap<Model.VendorCodeType, Domain.Model.VendorCodeType>();

            // Domain to Data Mappings
            CreateMap<Domain.Model.Answer, Model.Answer>();
            CreateMap<Domain.Model.AuthorizationCode, Model.AuthorizationCode>();
            CreateMap<Domain.Model.AvatarBundle, Model.AvatarBundle>();
            CreateMap<Domain.Model.AvatarColor, Model.AvatarColor>();
            CreateMap<Domain.Model.AvatarElement, Model.AvatarElement>();
            CreateMap<Domain.Model.AvatarItem, Model.AvatarItem>();
            CreateMap<Domain.Model.AvatarLayer, Model.AvatarLayer>();
            CreateMap<Domain.Model.Badge, Model.Badge>();
            CreateMap<Domain.Model.Book, Model.Book>();
            CreateMap<Domain.Model.Branch, Model.Branch>();
            CreateMap<Domain.Model.Broadcast, Model.Broadcast>();
            CreateMap<Domain.Model.Carousel, Model.Carousel>();
            CreateMap<Domain.Model.CarouselItem, Model.CarouselItem>();
            CreateMap<Domain.Model.Category, Model.Category>();
            CreateMap<Domain.Model.Challenge, Model.Challenge>();
            CreateMap<Domain.Model.ChallengeGroup, Model.ChallengeGroup>();
            CreateMap<Domain.Model.ChallengeTask, Model.ChallengeTask>();
            CreateMap<Domain.Model.DailyLiteracyTip, Model.DailyLiteracyTip>();
            CreateMap<Domain.Model.DailyLiteracyTipImage, Model.DailyLiteracyTipImage>();
            CreateMap<Domain.Model.DashboardContent, Model.DashboardContent>();
            CreateMap<Domain.Model.DirectEmailHistory, Model.DirectEmailHistory>();
            CreateMap<Domain.Model.DirectEmailTemplate, Model.DirectEmailTemplate>();
            CreateMap<Domain.Model.DirectEmailTemplateText, Model.DirectEmailTemplateText>();
            CreateMap<Domain.Model.Drawing, Model.Drawing>();
            CreateMap<Domain.Model.DrawingCriterion, Model.DrawingCriterion>();
            CreateMap<Domain.Model.EmailBase, Model.EmailBase>();
            CreateMap<Domain.Model.EmailBaseText, Model.EmailBaseText>();
            CreateMap<Domain.Model.EmailReminder, Model.EmailReminder>();
            CreateMap<Domain.Model.EmailSubscriptionAuditLog, Model.EmailSubscriptionAuditLog>();
            CreateMap<Domain.Model.Event, Model.Event>();
            CreateMap<Domain.Model.GroupInfo, Model.GroupInfo>();
            CreateMap<Domain.Model.GroupType, Model.GroupType>();
            CreateMap<Domain.Model.Job, Model.Job>();
            CreateMap<Domain.Model.Language, Model.Language>();
            CreateMap<Domain.Model.Location, Model.Location>();
            CreateMap<Domain.Model.Mail, Model.Mail>();
            CreateMap<Domain.Model.NewsCategory, Model.NewsCategory>();
            CreateMap<Domain.Model.NewsPost, Model.NewsPost>();
            CreateMap<Domain.Model.Notification, Model.Notification>();
            CreateMap<Domain.Model.Page, Model.Page>();
            CreateMap<Domain.Model.PageHeader, Model.PageHeader>();
            CreateMap<Domain.Model.PointTranslation, Model.PointTranslation>();
            CreateMap<Domain.Model.PrizeWinner, Model.PrizeWinner>();
            CreateMap<Domain.Model.Program, Model.Program>();
            CreateMap<Domain.Model.PsAgeGroup, Model.PsAgeGroup>();
            CreateMap<Domain.Model.PsBlackoutDate, Model.PsBlackoutDate>();
            CreateMap<Domain.Model.PsBranchSelection, Model.PsBranchSelection>();
            CreateMap<Domain.Model.PsKit, Model.PsKit>();
            CreateMap<Domain.Model.PsKitImage, Model.PsKitImage>();
            CreateMap<Domain.Model.PsPerformer, Model.PsPerformer>();
            CreateMap<Domain.Model.PsPerformerImage, Model.PsPerformerImage>();
            CreateMap<Domain.Model.PsPerformerSchedule, Model.PsPerformerSchedule>();
            CreateMap<Domain.Model.PsProgram, Model.PsProgram>();
            CreateMap<Domain.Model.PsProgramImage, Model.PsProgramImage>();
            CreateMap<Domain.Model.PsSettings, Model.PsSettings>();
            CreateMap<Domain.Model.Question, Model.Question>();
            CreateMap<Domain.Model.Questionnaire, Model.Questionnaire>();
            CreateMap<Domain.Model.RecoveryToken, Model.RecoveryToken>();
            CreateMap<Domain.Model.ReportCriterion, Model.ReportCriterion>();
            CreateMap<Domain.Model.ReportRequest, Model.ReportRequest>();
            CreateMap<Domain.Model.RequiredQuestionnaire, Model.RequiredQuestionnaire>();
            CreateMap<Domain.Model.Role, Model.Role>();
            CreateMap<Domain.Model.School, Model.School>();
            CreateMap<Domain.Model.SchoolDistrict, Model.SchoolDistrict>();
            CreateMap<Domain.Model.Site, Model.Site>();
            CreateMap<Domain.Model.SiteSetting, Model.SiteSetting>();
            CreateMap<Domain.Model.Social, Model.Social>();
            CreateMap<Domain.Model.SocialHeader, Model.SocialHeader>();
            CreateMap<Domain.Model.SpatialDistanceDetail, Model.SpatialDistanceDetail>();
            CreateMap<Domain.Model.SpatialDistanceHeader, Model.SpatialDistanceHeader>();
            CreateMap<Domain.Model.System, Model.System>();
            CreateMap<Domain.Model.Trigger, Model.Trigger>();
            CreateMap<Domain.Model.User, Model.User>();
            CreateMap<Domain.Model.UserLog, Model.UserLog>();
            CreateMap<Domain.Model.VendorCode, Model.VendorCode>();
            CreateMap<Domain.Model.VendorCodePackingSlip, Model.VendorCodePackingSlip>();
            CreateMap<Domain.Model.VendorCodeType, Model.VendorCodeType>();
        }
    }
}
