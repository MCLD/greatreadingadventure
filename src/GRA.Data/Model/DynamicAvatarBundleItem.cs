namespace GRA.Data.Model
{
    public class DynamicAvatarBundleItem
    {
        public int DynamicAvatarBundleId { get; set; }
        public DynamicAvatarBundle DynamicAvatarBundle { get; set; }

        public int DynamicAvatarItemId { get; set; }
        public DynamicAvatarItem DynamicAvatarItem { get; set; }
    }
}
