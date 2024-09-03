using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Test3enc.Models;
using Test3enc.Services.IServices;

public class RsaEncryptionService : IRsaEncryptionService
{
    private readonly IKeyRepository _keyRepository;

    public RsaEncryptionService(IKeyRepository keyRepository)
    {
        _keyRepository = keyRepository;
    }

    public async Task<EncryptedFile> EncryptFileAsync(IFormFile file, byte[] publicKey)
    {
        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPublicKey(publicKey, out _);

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                // Encrypt the file data using RSA
                byte[] encryptedData = rsa.Encrypt(fileBytes, RSAEncryptionPadding.OaepSHA256);

                return new EncryptedFile
                {
                    FileName = file.FileName,
                    EncryptedData = encryptedData,
                    EncryptionAlgorithm = "RSA"
                };
            }
        }
    }

    public async Task<byte[]> DecryptFileAsync(EncryptedFile encryptedFile)
    {
        using (var rsa = RSA.Create())
        {
            try
            {
                // Load the RSA private key from the repository
                string privateKeyBase64 = await _keyRepository.GetRsaPrivateKeyAsync();
                if (string.IsNullOrEmpty(privateKeyBase64))
                {
                    throw new InvalidOperationException("RSA private key is not available.");
                }

                byte[] privateKey = Convert.FromBase64String(privateKeyBase64);
                rsa.ImportRSAPrivateKey(privateKey, out _);

                // Decrypt the file data using RSA
                byte[] decryptedData = rsa.Decrypt(encryptedFile.EncryptedData, RSAEncryptionPadding.OaepSHA256);

                return decryptedData;
            }
            catch (CryptographicException ex)
            {
                // Log specific details about the exception
                Console.WriteLine($"Cryptographic exception during decryption: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log other exceptions
                Console.WriteLine($"Exception during decryption: {ex.Message}");
                throw;
            }
        }
    }
}
