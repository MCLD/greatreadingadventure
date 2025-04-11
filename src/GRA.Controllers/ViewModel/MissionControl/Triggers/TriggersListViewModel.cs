using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Triggers
{
    public class TriggersListViewModel
    {
        public string ActiveNav { get; set; }
        public int? BranchId { get; set; }
        public IEnumerable<Domain.Model.Branch> BranchList { get; set; }
        public string BranchName { get; set; }
        public bool? HideLowPoint { get; set; }
        public bool? LowPoints { get; set; }
        public bool? Mine { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? ProgramId { get; set; }
        public IEnumerable<Domain.Model.Program> ProgramList { get; set; }
        public string ProgramName { get; set; }
        public string Search { get; set; }
        public int? SystemId { get; set; }
        public IEnumerable<Domain.Model.System> SystemList { get; set; }
        public string SystemName { get; set; }
        public IEnumerable<Domain.Model.Trigger> Triggers { get; set; }

        public static string DisplayActivation(Domain.Model.Trigger trigger)
        {
            ArgumentNullException.ThrowIfNull(trigger);
            if (!string.IsNullOrEmpty(trigger.SecretCode))
            {
                return "secret code";
            }
            var sb = new StringBuilder();
            if (trigger.ItemsRequired.HasValue
                && (trigger.ChallengeIds?.Count > 0 || trigger.BadgeIds?.Count > 0))
            {
                sb.Append(trigger.ItemsRequired.Value);
                if (trigger.ChallengeIds?.Count > 0)
                {
                    sb.Append(" challenge");
                    if (trigger.ItemsRequired.Value != 1)
                    {
                        sb.Append('s');
                    }
                    if (trigger.BadgeIds?.Count > 0)
                    {
                        sb.Append("/trigger");
                        if (trigger.ItemsRequired.Value != 1)
                        {
                            sb.Append('s');
                        }
                    }
                }
                if (trigger.BadgeIds?.Count > 0)
                {
                    sb.Append(" trigger");
                    if (trigger.ItemsRequired.Value != 1)
                    {
                        sb.Append('s');
                    }
                }
                if (trigger.Points > 0)
                {
                    sb.Append(" & ");
                }
            }
            if (trigger.Points > 0)
            {
                sb.Append(trigger.Points)
                    .Append(" points");
            }
            return sb.ToString();
        }

        public static string DisplayLimitations(Domain.Model.Trigger trigger)
        {
            if (trigger != null)
            {
                var sb = new StringBuilder();
                if (trigger.LimitToSystemId.HasValue)
                {
                    sb.Append("system, ");
                }
                if (trigger.LimitToBranchId.HasValue)
                {
                    sb.Append("branch, ");
                }
                if (trigger.LimitToProgramId.HasValue)
                {
                    sb.Append("program");
                }
                if (sb.Length > 0)
                {
                    return "<div><em><small>Limited by: "
                        + sb.ToString().Trim().Trim(',')
                        + "</small></em></div>";
                }
            }
            return string.Empty;
        }
    }
}
