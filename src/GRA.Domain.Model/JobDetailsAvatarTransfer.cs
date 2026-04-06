namespace GRA.Domain.Model
{
    public class JobDetailsAvatarTransfer
    {
        public string AssetPath { get; set; }
        public string Filename { get; set; }
        public long? Filesize { get; set; }
        public DataTransferType TransferType { get; set; }
        public bool UploadedFile { get; set; }
        public int Version { get; set; }
    }
}
