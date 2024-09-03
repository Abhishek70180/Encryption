using Test3enc.Models;

namespace Test3enc.Services.IServices
{
    public interface ITripleDesEncryptionService
    {
        public Task<EncryptedFile> EncryptFileAsync(IFormFile file, byte[] key);
        public Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile);
    }
}
