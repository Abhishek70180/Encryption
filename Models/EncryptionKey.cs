namespace Test3enc.Models
{
    public class EncryptionKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Key { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
