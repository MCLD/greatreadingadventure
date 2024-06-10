using System.Collections.Generic;
using System.Globalization;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class VendorCodeViewModel : ParticipantPartialViewModel
    {
        public VendorCodeViewModel()
        {
        }

        public VendorCodeViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public IEnumerable<VendorCodeInfo> AssociatedCodes { get; set; }

        public IEnumerable<VendorCodeInfo> CurrentCodes { get; set; }

        public bool CurrentUser { get; set; }
        public SelectList VendorCodeTypeList { get; set; }

        public static string CurrentCodeClass(int? currentCodeCount)
        {
            return currentCodeCount == 1 ? "success" : "danger";
        }

        public static string GetFormattedDates(VendorCodeInfo vendorCodeInfo)
        {
            var sb = new System.Text.StringBuilder();
            if (vendorCodeInfo?.VendorCode?.OrderDate.HasValue == true)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture,
                    "Order date: {0:d}",
                    vendorCodeInfo.VendorCode.OrderDate.Value);
            }
            if (vendorCodeInfo?.VendorCode?.ShipDate.HasValue == true)
            {
                if (sb.Length > 0)
                {
                    sb.Append("<br>");
                }
                sb.AppendFormat(CultureInfo.InvariantCulture,
                    "Ship date: {0:d}",
                    vendorCodeInfo.VendorCode.ShipDate.Value);
            }
            if (vendorCodeInfo?.PickupDate.HasValue == true)
            {
                if (sb.Length > 0)
                {
                    sb.Append("<br>");
                }
                sb.AppendFormat(CultureInfo.InvariantCulture,
                    "<strong>Pick-up date: {0:d}</strong>",
                    vendorCodeInfo.PickupDate);
            }
            return sb.ToString();
        }

        public static string GetFormattedNotes(VendorCodeInfo codeInfo)
        {
            if (codeInfo == null)
            {
                return null;
            }

            var sb = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(codeInfo?.VendorCodeMessage))
            {
                sb.Append("<div>").Append(codeInfo.VendorCodeMessage).Append("</div>");
            }
            if (!string.IsNullOrEmpty(codeInfo?.VendorCode?.PackingSlip))
            {
                sb.Append("<div>Packing slip: <a href=\"")
                    .Append(codeInfo.PackingSlipLink)
                    .Append("\"><strong>")
                    .Append(codeInfo.VendorCode.PackingSlip)
                    .Append("</strong></a></div>");
            }
            if (!string.IsNullOrEmpty(codeInfo?.VendorCode?.ReasonForReassignment))
            {
                sb.Append("<div>Reason/explanation: ")
                    .Append(codeInfo.VendorCode.ReasonForReassignment)
                    .Append("</div>");
            }
            if (!string.IsNullOrEmpty(codeInfo?.ReassignedByUser))
            {
                sb.Append("<div>Assigned by: <strong><a href=\"")
                    .Append(codeInfo.ReassignedByLink)
                    .Append("\">")
                    .Append(codeInfo.ReassignedByUser)
                    .Append("</a></strong> on <strong>")
                    .Append(codeInfo.VendorCode.ReassignedAt)
                    .Append("</strong></div>");
            }
            if (codeInfo.IsDamaged)
            {
                sb.Append("<span class=\"label label-danger\">Damaged</span>");
            }
            if (codeInfo.IsMissing)
            {
                sb.Append("<span class=\"label label-warning\">Missing</span>");
            }
            return sb.ToString();
        }
    }
}
