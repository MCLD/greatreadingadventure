using System.Collections.Generic;
using GRA.Domain.Model.Report;

namespace GRA.Controllers.ViewModel.MissionControl.VendorCodes
{
    public class ShippingDetailsViewModel
    {
        public ShippingDetailsViewModel()
        {
            ItemStatuses = [];
        }

        public ICollection<VendorCodeItemStatus> ItemStatuses { get; }
        public int VendorCodeTypeId { get; set; }
    }
}
