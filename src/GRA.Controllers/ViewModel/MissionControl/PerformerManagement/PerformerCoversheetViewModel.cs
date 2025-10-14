
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerCoversheetViewModel
    {
        public string LibraryBranch { get; set; }
        public string StaffContact { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public DateTime ProgramDate { get; set; }
        public int PerformerId { get; set; }
        public string PerformerName {  get; set; }
        public string VendorId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PayToName { get; set; }
        public string PayToAddress { get; set; }
        public SelectList Months { get; set; }
    }
}
