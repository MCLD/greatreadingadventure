using System.Linq;
using Mapster;

namespace GRA.Data.Profile
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Data to Domain Mappings
            //CreateMap<Model.Answer, Domain.Model.Answer>();
            //CreateMap<Model.Attachment, Domain.Model.Attachment>();
            //CreateMap<Model.AuthorizationCode, Domain.Model.AuthorizationCode>();
            TypeAdapterConfig<Model.AvatarBundle, Domain.Model.AvatarBundle>.NewConfig()
                .Map(dest => dest.AvatarItems,
                    src => src.AvatarBundleItems.Select(_ => _.AvatarItem));

            //CreateMap<Model.AvatarColor, Domain.Model.AvatarColor>();
            //CreateMap<Model.AvatarElement, Domain.Model.AvatarElement>();
            //CreateMap<Model.AvatarItem, Domain.Model.AvatarItem>();
            TypeAdapterConfig<Model.AvatarLayer, Domain.Model.AvatarLayer>.NewConfig()
                .Map(dest => dest.AvatarColors, src => src.AvatarColors.OrderBy(_ => _.SortOrder)) //explicit expansion removed
                .Map(dest => dest.AvatarItems, src => src.AvatarItems.OrderBy(_ => _.SortOrder)); //explicit expansion removed

            //CreateMap<Model.Badge, Domain.Model.Badge>();
            //CreateMap<Model.Book, Domain.Model.Book>();
            //CreateMap<Model.Branch, Domain.Model.Branch>();
            //CreateMap<Model.Broadcast, Domain.Model.Broadcast>();
            //CreateMap<Model.Carousel, Domain.Model.Carousel>();
            //CreateMap<Model.CarouselItem, Domain.Model.CarouselItem>();
            //CreateMap<Model.Category, Domain.Model.Category>();
            TypeAdapterConfig<Model.Challenge, Domain.Model.Challenge>.NewConfig()
                .Map(dest => dest.Categories,
                    src => src.ChallengeCategories.Select(_ => _.Category));
            TypeAdapterConfig<Model.ChallengeGroup, Domain.Model.ChallengeGroup>.NewConfig()
                .Map(dest => dest.ChallengeIds,
                    src => src.ChallengeGroupChallenges.Select(_ => _.ChallengeId)); // .ToList() removed afer .Select
            //CreateMap<Model.ChallengeTask, Domain.Model.ChallengeTask>();
            //CreateMap<Model.DailyLiteracyTip, Domain.Model.DailyLiteracyTip>();
            //CreateMap<Model.DailyLiteracyTipImage, Domain.Model.DailyLiteracyTipImage>();
            //CreateMap<Model.DashboardContent, Domain.Model.DashboardContent>();
            //CreateMap<Model.DirectEmailHistory, Domain.Model.DirectEmailHistory>();
            //CreateMap<Model.DirectEmailTemplate, Domain.Model.DirectEmailTemplate>();
            //CreateMap<Model.DirectEmailTemplateText, Domain.Model.DirectEmailTemplateText>();
            //CreateMap<Model.Drawing, Domain.Model.Drawing>();
            TypeAdapterConfig<Model.DrawingCriterion, Domain.Model.DrawingCriterion>.NewConfig()
                .Map(dest => dest.ProgramIds,
                    src => src.CriterionPrograms.Select(_ => _.ProgramId)); // .ToList() removed afer .Select
            //CreateMap<Model.EmailBase, Domain.Model.EmailBase>();
            //CreateMap<Model.EmailBaseText, Domain.Model.EmailBaseText>();
            //CreateMap<Model.EmailReminder, Domain.Model.EmailReminder>();
            TypeAdapterConfig<Model.EmailSubscriptionAuditLog, Domain.Model.EmailSubscriptionAuditLog>.NewConfig()
                .Map(dest => dest.CreatedByName,
                    src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}");
            TypeAdapterConfig<Model.Event, Domain.Model.Event>.NewConfig()
                .Ignore(dest => dest.Challenge)
                .Ignore(dest => dest.ChallengeGroup);
            //CreateMap<Model.ExitLandingMessageSet, Domain.Model.ExitLandingMessageSet>();
            //CreateMap<Model.FeaturedChallengeGroup, Domain.Model.FeaturedChallengeGroup>();
            //CreateMap<Model.FeaturedChallengeGroupText, Domain.Model.FeaturedChallengeGroupText>();
            //CreateMap<Model.GroupInfo, Domain.Model.GroupInfo>();
            //CreateMap<Model.GroupType, Domain.Model.GroupType>();
            //CreateMap<Model.Job, Domain.Model.Job>();
            //CreateMap<Model.Language, Domain.Model.Language>();
            //CreateMap<Model.Location, Domain.Model.Location>();
            //CreateMap<Model.Mail, Domain.Model.Mail>();
            //CreateMap<Model.MessageTemplate, Domain.Model.MessageTemplate>();
            //CreateMap<Model.MessageTemplateText, Domain.Model.MessageTemplateText>();
            TypeAdapterConfig<Model.NewsCategory, Domain.Model.NewsCategory>.NewConfig()
                .Map(dest => dest.PostCount, src => src.Posts.Count());
            //CreateMap<Model.NewsPost, Domain.Model.NewsPost>();
            //CreateMap<Model.Notification, Domain.Model.Notification>();
            TypeAdapterConfig<Model.PageHeader, Domain.Model.PageHeader>.NewConfig()
                .Map(dest => dest.PageLanguages,
                    src => src.Pages.Select(_ => _.Language.Description));
            //CreateMap<Model.Page, Domain.Model.Page>();
            //CreateMap<Model.PointTranslation, Domain.Model.PointTranslation>();
            TypeAdapterConfig<Model.PrizeWinner, Domain.Model.PrizeWinner>.NewConfig()
                .Map(dest => dest.PrizeName, src => src.Drawing.Name ?? src.Trigger.AwardPrizeName);
            //CreateMap<Model.Program, Domain.Model.Program>();
            //CreateMap<Model.PsAgeGroup, Domain.Model.PsAgeGroup>();
            //CreateMap<Model.PsBlackoutDate, Domain.Model.PsBlackoutDate>();
            TypeAdapterConfig<Model.PsBranchSelection, Domain.Model.PsBranchSelection>.NewConfig()
                .Map(dest => dest.StartsAt, src => src.ScheduleStartTime.ToShortTimeString())
                .Map(dest => dest.EndsAt, src => src.ScheduleStartTime.AddMinutes(src.ScheduleDuration)
                    .ToShortTimeString());
            //CreateMap<Model.PsSettings, Domain.Model.PsSettings>();
            //CreateMap<Model.PsKit, Domain.Model.PsKit>();
            //CreateMap<Model.PsKitImage, Domain.Model.PsKitImage>();
            //CreateMap<Model.PsPerformer, Domain.Model.PsPerformer>();
            //CreateMap<Model.PsPerformerImage, Domain.Model.PsPerformerImage>();
            //CreateMap<Model.PsPerformerSchedule, Domain.Model.PsPerformerSchedule>();
            //CreateMap<Model.PsProgram, Domain.Model.PsProgram>();
            //CreateMap<Model.PsProgramImage, Domain.Model.PsProgramImage>();
            TypeAdapterConfig<Model.Question, Domain.Model.Question>.NewConfig()
                .Map(dest => dest.Answers, src => src.Answers.OrderBy(_ => _.SortOrder)); //explicit expansion removed
            TypeAdapterConfig<Model.Questionnaire, Domain.Model.Questionnaire>.NewConfig()
                .Map(dest => dest.Questions, 
                    src => src.Questions.Where(_ => !_.IsDeleted).OrderBy(_ => _.SortOrder));
            //CreateMap<Model.RecoveryToken, Domain.Model.RecoveryToken>();
            //CreateMap<Model.ReportCriterion, Domain.Model.ReportCriterion>();
            //CreateMap<Model.ReportRequest, Domain.Model.ReportRequest>();
            //CreateMap<Model.RequiredQuestionnaire, Domain.Model.RequiredQuestionnaire>();
            //CreateMap<Model.Role, Domain.Model.Role>();
            //CreateMap<Model.School, Domain.Model.School>();
            //CreateMap<Model.SchoolDistrict, Domain.Model.SchoolDistrict>();
            //CreateMap<Model.Segment, Domain.Model.Segment>();
            //CreateMap<Model.SegmentText, Domain.Model.SegmentText>();
            //CreateMap<Model.Site, Domain.Model.Site>();
            //CreateMap<Model.SiteSetting, Domain.Model.SiteSetting>();
            //CreateMap<Model.Social, Domain.Model.Social>();
            //CreateMap<Model.SocialHeader, Domain.Model.SocialHeader>();
            //CreateMap<Model.SpatialDistanceDetail, Domain.Model.SpatialDistanceDetail>();
            //CreateMap<Model.SpatialDistanceHeader, Domain.Model.SpatialDistanceHeader>();
            TypeAdapterConfig<Model.System, Domain.Model.System>.NewConfig()
                .Map(dest => dest.Branches, src => src.Branches.OrderBy(_ => _.Name));
            TypeAdapterConfig<Model.Trigger, Domain.Model.Trigger>.NewConfig()
                .Map(dest => dest.BadgeIds, src => src.RequiredBadges.Select(_ => _.BadgeId)) // .ToList() removed afer .Select
                .Map(dest => dest.ChallengeIds, 
                    src => src.RequiredChallenges.Select(_ => _.ChallengeId));// .ToList() removed afer .Select
            //CreateMap<Model.User, Domain.Model.User>();
            //CreateMap<Model.UserLog, Domain.Model.UserLog>();
            //CreateMap<Model.VendorCode, Domain.Model.VendorCode>();
            //CreateMap<Model.VendorCodePackingSlip, Domain.Model.VendorCodePackingSlip>();
            //CreateMap<Model.VendorCodeType, Domain.Model.VendorCodeType>();

            // Domain to Data Mappings
            /*
            CreateMap<Domain.Model.Answer, Model.Answer>();
            CreateMap<Domain.Model.Attachment, Model.Attachment>();
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
            CreateMap<Domain.Model.ExitLandingMessageSet, Model.ExitLandingMessageSet>();
            CreateMap<Domain.Model.FeaturedChallengeGroup, Model.FeaturedChallengeGroup>();
            CreateMap<Domain.Model.FeaturedChallengeGroupText, Model.FeaturedChallengeGroupText>();
            CreateMap<Domain.Model.GroupInfo, Model.GroupInfo>();
            CreateMap<Domain.Model.GroupType, Model.GroupType>();
            CreateMap<Domain.Model.Job, Model.Job>();
            CreateMap<Domain.Model.Language, Model.Language>();
            CreateMap<Domain.Model.Location, Model.Location>();
            CreateMap<Domain.Model.Mail, Model.Mail>();
            CreateMap<Domain.Model.MessageTemplate, Model.MessageTemplate>();
            CreateMap<Domain.Model.MessageTemplateText, Model.MessageTemplateText>();
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
            CreateMap<Domain.Model.Segment, Model.Segment>();
            CreateMap<Domain.Model.SegmentText, Model.SegmentText>();
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
            */
        }
    }
}
