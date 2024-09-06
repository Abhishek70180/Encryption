using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Test3enc.Models;
using Test3enc.Services.IServices;

namespace Test3enc.Services
{
    public class RsaEncryptionService : IRsaEncryptionService
    {
        private const int KeySize = 2048;

        public (string PublicKey, string PrivateKey) GenerateRsaKeyPair()
        {
            using (var rsa = RSA.Create(KeySize))
            {
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                return (PublicKey: publicKey, PrivateKey: privateKey);
            }
        }

        public async Task<EncryptedFile> EncryptFileAsync(IFormFile file, string publicKey)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.GenerateKey();
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        var encryptedFileBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
                        var rsaEncryptedKey = EncryptAesKeyWithRsa(aes.Key, publicKey);

                        return new EncryptedFile
                        {
                            FileName = file.FileName,
                            EncryptedData = encryptedFileBytes,
                            EncryptionAlgorithm = "RSA+AES",
                            EncryptionKey = rsaEncryptedKey,
                            IV = aes.IV,
                            UploadedAt = DateTime.UtcNow
                        };
                    }
                }
            }
        }

        public async Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile, string privateKey)
        {
            var aesKey = DecryptAesKeyWithRsa(encryptedFile.EncryptionKey, privateKey);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = aesKey;
                aes.IV = encryptedFile.IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var decryptedFileBytes = decryptor.TransformFinalBlock(encryptedFile.EncryptedData, 0, encryptedFile.EncryptedData.Length);
                    return decryptedFileBytes;
                }
            }
        }

        private byte[] EncryptAesKeyWithRsa(byte[] aesKey, string publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                return rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            }
        }

        private byte[] DecryptAesKeyWithRsa(byte[] encryptedAesKey, string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                return rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);
            }
        }
    }
}
