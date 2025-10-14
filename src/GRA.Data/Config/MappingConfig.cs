using System.Linq;
using Mapster;

namespace GRA.Data.Config
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Data to Domain Mappings
            TypeAdapterConfig<Model.AvatarBundle, Domain.Model.AvatarBundle>.NewConfig()
                .IgnoreIf((src, _) => src.AvatarBundleItems == null, dest => dest.AvatarItems)
                .Map(dest => dest.AvatarItems,
                    src => src.AvatarBundleItems.Select(_ => _.AvatarItem));

            // Forked in AvatarLayerRepository
            TypeAdapterConfig<Model.AvatarLayer, Domain.Model.AvatarLayer>.NewConfig()
                .Ignore(dest => dest.AvatarColors)
                .Ignore(dest => dest.AvatarItems);

            TypeAdapterConfig<Model.Challenge, Domain.Model.Challenge>.NewConfig()
                .IgnoreIf((src, _) => src.ChallengeCategories == null, dest => dest.Categories)
                .Map(dest => dest.Categories,
                    src => src.ChallengeCategories.Select(_ => _.Category));

            TypeAdapterConfig<Model.ChallengeGroup, Domain.Model.ChallengeGroup>.NewConfig()
                .IgnoreIf((src, _) => src.ChallengeGroupChallenges == null,
                    dest => dest.ChallengeIds)
                .Map(dest => dest.ChallengeIds,
                    src => src.ChallengeGroupChallenges.Select(_ => _.ChallengeId));

            TypeAdapterConfig<Model.DrawingCriterion, Domain.Model.DrawingCriterion>.NewConfig()
                .IgnoreIf((src, _) => src.CriterionPrograms == null, dest => dest.ProgramIds)
                .Map(dest => dest.ProgramIds,
                    src => src.CriterionPrograms.Select(_ => _.ProgramId));

            TypeAdapterConfig<Model.EmailSubscriptionAuditLog,
                Domain.Model.EmailSubscriptionAuditLog>.NewConfig()
                .IgnoreIf((src, _) => src.CreatedByUser == null, dest => dest.CreatedByName)
                    .Map(dest => dest.CreatedByName,
                        src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}");

            TypeAdapterConfig<Model.Event, Domain.Model.Event>.NewConfig()
                .Ignore(dest => dest.Challenge)
                .Ignore(dest => dest.ChallengeGroup);

            TypeAdapterConfig<Model.NewsCategory, Domain.Model.NewsCategory>.NewConfig()
                .IgnoreIf((src, _) => src.Posts == null, dest => dest.PostCount)
                .Map(dest => dest.PostCount, src => src.Posts.Count);

            TypeAdapterConfig<Model.PageHeader, Domain.Model.PageHeader>.NewConfig()
                .IgnoreIf((src, _) => src.Pages == null, dest => dest.PageLanguages)
                .Map(dest => dest.PageLanguages,
                    src => src.Pages.Select(_ => _.Language.Description));

            TypeAdapterConfig<Model.PrizeWinner, Domain.Model.PrizeWinner>.NewConfig()
                .IgnoreIf((src, _) => src.Drawing == null && src.Trigger == null,
                    dest => dest.PrizeName)
                .Map(dest => dest.PrizeName, src => src.Drawing.Name ?? src.Trigger.AwardPrizeName);

            TypeAdapterConfig<Model.PsBranchSelection, Domain.Model.PsBranchSelection>.NewConfig()
                .Map(dest => dest.StartsAt, src => src.ScheduleStartTime.ToShortTimeString())
                .Map(dest => dest.EndsAt,
                    src => src.ScheduleStartTime.AddMinutes(src.ScheduleDuration)
                        .ToShortTimeString());

            // Forked in QuestionnaireRepository and QuestionRepository
            TypeAdapterConfig<Model.Question, Domain.Model.Question>.NewConfig()
                .Ignore(dest => dest.Answers);

            TypeAdapterConfig<Model.Questionnaire, Domain.Model.Questionnaire>.NewConfig()
                .IgnoreIf((src, _) => src.Questions == null, dest => dest.Questions)
                .Map(dest => dest.Questions,
                    src => src.Questions.Where(_ => !_.IsDeleted).OrderBy(_ => _.SortOrder));

            TypeAdapterConfig<Model.System, Domain.Model.System>.NewConfig()
                .IgnoreIf((src, _) => src.Branches == null, dest => dest.Branches)
                .Map(dest => dest.Branches, src => src.Branches.OrderBy(_ => _.Name));

            TypeAdapterConfig<Model.Trigger, Domain.Model.Trigger>.NewConfig()
                .IgnoreIf((src, _) => src.RequiredBadges == null, dest => dest.BadgeIds)
                .Map(dest => dest.BadgeIds, src => src.RequiredBadges.Select(_ => _.BadgeId))
                .IgnoreIf((src, _) => src.RequiredChallenges == null, dest => dest.ChallengeIds)
                .Map(dest => dest.ChallengeIds,
                    src => src.RequiredChallenges.Select(_ => _.ChallengeId));
        }
    }
}
