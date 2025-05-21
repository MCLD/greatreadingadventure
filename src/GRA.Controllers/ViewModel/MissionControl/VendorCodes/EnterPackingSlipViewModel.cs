using System;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.VendorCodes
{
    public class EnterPackingSlipViewModel
    {
        public string PackingSlipNumber { get; set; }
        public IDictionary<string, DateTime> ViewedPackingSlips { get; set; }
    }
}
