namespace Test3enc.Models
{
    public class EncryptedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] EncryptedData { get; set; }
        public string EncryptionAlgorithm { get; set; }
        public byte[] IV { get; set; }
        public byte[] EncryptionIv { get; set; }
        public byte[] EncryptionKey { get; set; }
        public byte[] EncryptedKey { get; set; }
        public DateTime UploadedAt { get; set; }
        public string PrivateKey { get; internal set; }
    }
}
