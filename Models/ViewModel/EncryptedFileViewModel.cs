namespace Test3enc.Models.ViewModel
{
    public class EncryptedFileViewModel
    {
        public IEnumerable<EncryptedFile> EncryptedFiles { get; set; }
        public IEnumerable<EncryptionKey> EncryptionKeys { get; set; }
    }
}
