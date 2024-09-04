using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Test3enc.Models;
using Test3enc.Services.IServices;

namespace Test3enc.Services
{
    public class TripleDesEncryptionService : ITripleDesEncryptionService
    {
        private const int TripleDesKeySize = 192;
        private const int TripleDesBlockSize = 64;

        public async Task<EncryptedFile> EncryptFileAsync(IFormFile file, byte[] key)
        {
            if (key.Length != TripleDesKeySize / 8)
            {
                throw new ArgumentException("Invalid key size for TripleDES encryption.");
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                using (var tripledes = TripleDES.Create())
                {
                    tripledes.KeySize = TripleDesKeySize;
                    tripledes.BlockSize = TripleDesBlockSize;
                    tripledes.Mode = CipherMode.CBC;
                    tripledes.Padding = PaddingMode.PKCS7;
                    tripledes.Key = key;
                    tripledes.GenerateIV();

                    using (var encryptor = tripledes.CreateEncryptor(tripledes.Key, tripledes.IV))
                    {
                        var encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
                        return new EncryptedFile
                        {
                            FileName = file.FileName,
                            EncryptedData = encryptedBytes,
                            EncryptionKey = tripledes.Key,
                            EncryptionIv = tripledes.IV,
                            EncryptionAlgorithm = "TripleDES"
                        };
                    }
                }
            }
        }

        public async Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile)
        {
            if (encryptedFile.EncryptionKey.Length != TripleDesKeySize / 8 ||
                encryptedFile.EncryptionIv.Length != TripleDesBlockSize / 8)
            {
                throw new ArgumentException("Invalid key or IV size for TripleDES decryption.");
            }

            using (var tripledes = TripleDES.Create())
            {
                tripledes.KeySize = TripleDesKeySize;
                tripledes.BlockSize = TripleDesBlockSize;
                tripledes.Mode = CipherMode.CBC;
                tripledes.Padding = PaddingMode.PKCS7;
                tripledes.Key = encryptedFile.EncryptionKey;
                tripledes.IV = encryptedFile.EncryptionIv;

                using (var decryptor = tripledes.CreateDecryptor(tripledes.Key, tripledes.IV))
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedFile.EncryptedData, 0, encryptedFile.EncryptedData.Length);
                    return decryptedBytes;
                }
            }
        }
    }
}
