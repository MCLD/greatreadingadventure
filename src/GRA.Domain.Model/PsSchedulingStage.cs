namespace GRA.Domain.Model
{
    public enum PsSchedulingStage
    {
        Unavailable,
        BeforeRegistration,
        RegistrationOpen,
        RegistrationClosed,
        SchedulingPreview,
        SchedulingOpen,
        SchedulingClosed,
        SchedulePosted
    }
}
