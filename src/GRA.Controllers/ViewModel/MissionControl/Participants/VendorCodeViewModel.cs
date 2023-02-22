using System.Collections.Generic;
using System.Globalization;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class VendorCodeViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<VendorCodeInfo> AssociatedCodes { get; set; }
        public VendorCodeInfo CurrentCode { get; set; }

        public static string GetOrderShipDate(VendorCodeInfo vendorCodeInfo)
        {
                var sb = new System.Text.StringBuilder();
                if (vendorCodeInfo?.VendorCode?.OrderDate.HasValue == true)
                {
                    sb.Append("Order date: ")
                        .AppendFormat(CultureInfo.InvariantCulture,
                            "{0:d}",
                            vendorCodeInfo.VendorCode.OrderDate.Value);
                    if (vendorCodeInfo?.VendorCode?.ShipDate.HasValue == true)
                    {
                        sb.Append("<br>");
                    }
                }
                if (vendorCodeInfo?.VendorCode?.ShipDate.HasValue == true)
                {
                    sb.Append("Ship date: ")
                        .AppendFormat(CultureInfo.InvariantCulture,
                            "{0:d}",
                            vendorCodeInfo.VendorCode.ShipDate.Value);
                }
                return sb.ToString();
            }
    }
}
