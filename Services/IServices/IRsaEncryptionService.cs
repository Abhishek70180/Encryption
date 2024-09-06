using Test3enc.Models;

namespace Test3enc.Services.IServices
{
    public interface IRsaEncryptionService
    {
        Task<EncryptedFile> EncryptFileAsync(IFormFile file, string publicKey);
        Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile, string privateKey);
        (string PublicKey, string PrivateKey) GenerateRsaKeyPair();
    }
}
