using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.VendorCodes
{
    public class ConfigurationViewModel
    {
        public VendorCodeType VendorCodeType { get; set; }
        public SelectList PackingSlipOptions { get; set; }
        public SelectList ShipDateOptions { get; set; }
    }
}
