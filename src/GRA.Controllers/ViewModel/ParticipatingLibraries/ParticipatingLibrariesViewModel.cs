using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.ParticipatingBranches
{
    public class ParticipatingLibrariesViewModel
    {
        public IEnumerable<Domain.Model.System> Systems { get; set; }

        public static string FormatPhoneAddress(Domain.Model.Branch branch)
        {
            var sb = new System.Text.StringBuilder();
            if (branch != null &&
                (!string.IsNullOrEmpty(branch.Telephone) || !string.IsNullOrEmpty(branch.Address)))
            {
                if (branch.Telephone.Length > 0)
                {
                    sb.Append("<strong>").Append(branch.Telephone).Append("</strong> - ");
                }
                sb.Append(branch.Address?.Trim());
            }
            return sb.Length > 0 ? sb.ToString() : null;
        }
    }
}
