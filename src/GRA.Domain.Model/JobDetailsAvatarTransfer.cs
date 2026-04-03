namespace GRA.Domain.Model
{
    public class JobDetailsAvatarTransfer
    {
        public string AssetPath { get; set; }
        public DataTransferType TransferType { get; set; }
        public bool UploadedFile { get; set; }
        public int Version { get; set; }
    }
}
