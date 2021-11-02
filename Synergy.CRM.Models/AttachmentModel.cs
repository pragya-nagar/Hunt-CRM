namespace Synergy.CRM.Models
{
    public class AttachmentModel
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        public long Length { get; set; }
    }
}
