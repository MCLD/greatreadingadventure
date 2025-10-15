namespace GRA
{
    public readonly struct TemplateToken : System.IEquatable<TemplateToken>
    {
        public static readonly string VendorBranchToken = "BranchId";
        public static readonly string VendorCodeToken = "Code";
        public static readonly string VendorLinkToken = "Link";

        public bool Equals(TemplateToken other) => true;
    }
}
