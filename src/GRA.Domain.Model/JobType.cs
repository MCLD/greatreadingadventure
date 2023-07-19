namespace GRA.Domain.Model
{
    public enum JobType
    {
        None = 0,
        SendBulkEmails,
        RunReport,
        UpdateVendorStatus,
        AvatarImport,
        HouseholdImport,
        GenerateVendorCodes,
        UpdateEmailAwardStatus,
        BranchImport,
        SendNewsEmails,
        ReceivePackingSlip,
        BulkReassignCodes
    }
}
