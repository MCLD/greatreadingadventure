namespace GRA.Data.Model
{
    public class AvatarBundleItem
    {
        public int AvatarBundleId { get; set; }
        public AvatarBundle AvatarBundle { get; set; }

        public int AvatarItemId { get; set; }
        public AvatarItem AvatarItem { get; set; }
    }
}
