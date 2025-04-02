namespace GRA.Domain.Model
{
    public class ExitLandingMessageSet : Abstract.BaseDomainEntity
    {
        public int ExitLeftMessage { get; set; }
        public int LandingCenterMessage { get; set; }
        public int LandingLeftMessage { get; set; }
        public int LandingRightMessage { get; set; }
    }
}
