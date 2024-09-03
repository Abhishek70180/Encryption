using System.Security.Cryptography;
using Test3enc.Models;
using Test3enc.Services.IServices;

namespace Test3enc.Services
{
    public class AesEncryptionService : IAesEncryptionService
    {
        private const int AesKeySize = 256;
        private const int AesBlockSize = 128;

        public async Task<EncryptedFile> EncryptFileAsync(IFormFile file, byte[] key)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                using (var aes = Aes.Create())
                {
                    aes.KeySize = AesKeySize;
                    aes.BlockSize = AesBlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        var encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
                        return new EncryptedFile
                        {
                            FileName = file.FileName,
                            EncryptedData = encryptedBytes,
                            EncryptionKey = key,
                            IV = aes.IV
                        };
                    }
                }
            }
        }

        public async Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = AesKeySize;
                aes.BlockSize = AesBlockSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encryptedFile.EncryptionKey;
                aes.IV = encryptedFile.IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedFile.EncryptedData, 0, encryptedFile.EncryptedData.Length);
                    return decryptedBytes;
                }
            }
        }
    }
}