using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Test3enc.Models;
using Test3enc.Services.IServices;
using Test3enc.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace Test3enc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAesEncryptionService _aesEncryptionService;
        private readonly IRsaEncryptionService _rsaEncryptionService;
        private readonly ITripleDesEncryptionService _tripleDesEncryptionService;
        private readonly IEncryptedFileService _encryptedFileService;
        private readonly IKeyRepository _keyRepository;

        public HomeController(
            IAesEncryptionService aesEncryptionService,
            IRsaEncryptionService rsaEncryptionService,
            ITripleDesEncryptionService tripleDesEncryptionService,
            IEncryptedFileService encryptedFileService,
            IKeyRepository keyRepository)
        {
            _aesEncryptionService = aesEncryptionService;
            _rsaEncryptionService = rsaEncryptionService;
            _tripleDesEncryptionService = tripleDesEncryptionService;
            _encryptedFileService = encryptedFileService;
            _keyRepository = keyRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new EncryptedFileViewModel
            {
                EncryptedFiles = await _encryptedFileService.GetAllEncryptedFilesAsync(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string algorithm)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file to upload.");
                return RedirectToAction("Index");
            }

            byte[] key;
            EncryptedFile encryptedFile;

            try
            {
                switch (algorithm)
                {
                    case "AES":
                        key = GenerateAesKey();
                        encryptedFile = await _aesEncryptionService.EncryptFileAsync(file, key);
                        break;
                    case "RSA":
                        (key, _) = GenerateRsaKeyPair();
                        encryptedFile = await _rsaEncryptionService.EncryptFileAsync(file, key);
                        break;
                    case "TripleDES":
                        key = GenerateTripleDesKey();
                        encryptedFile = await _tripleDesEncryptionService.EncryptFileAsync(file, key);
                        break;
                    default:
                        throw new ArgumentException("Invalid encryption algorithm");
                }
                encryptedFile.UploadedAt = DateTime.UtcNow;
                encryptedFile.EncryptionAlgorithm = algorithm;
                await _encryptedFileService.SaveEncryptedFileAsync(encryptedFile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error during encryption: {ex.Message}";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "File encrypted and uploaded successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecryptAndDelete(int id)
        {
            var encryptedFile = await _encryptedFileService.GetEncryptedFileByIdAsync(id);
            if (encryptedFile == null)
            {
                return Json(new { success = false, message = "File not found." });
            }

            byte[] decryptedData;
            try
            {
                switch (encryptedFile.EncryptionAlgorithm)
                {
                    case "AES":
                        decryptedData = await _aesEncryptionService.DecryptFileAsync(encryptedFile);
                        break;
                    case "RSA":                      
                        decryptedData = await _rsaEncryptionService.DecryptFileAsync(encryptedFile);
                        break;
                    case "TripleDES":
                        decryptedData = await _tripleDesEncryptionService.DecryptFileAsync(encryptedFile);
                        break;
                    default:
                        throw new ArgumentException("Invalid encryption algorithm");
                }

                await _encryptedFileService.DeleteEncryptedFileAsync(id);

                string base64String = Convert.ToBase64String(decryptedData);

                // Return the file as a Base64 string
                return Json(new { success = true, message = "File decrypted and deleted successfully.", fileName = encryptedFile.FileName, fileContent = base64String });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error during decryption or deletion: {ex.Message}" });
            }
        }


        private byte[] GenerateAesKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private (byte[] publicKey, byte[] privateKey) GenerateRsaKeyPair()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                var publicKey = rsa.ExportRSAPublicKey();
                var privateKey = rsa.ExportRSAPrivateKey();
                return (publicKey, privateKey);
            }
        }

        private byte[] GenerateTripleDesKey()
        {
            using (var tripleDes = TripleDES.Create())
            {
                tripleDes.KeySize = 192;
                tripleDes.GenerateKey();
                return tripleDes.Key;
            }
        }
        private async Task<byte[]> LoadRsaPrivateKeyAsync()
        {
            var privateKeyBase64 = await _keyRepository.GetRsaPrivateKeyAsync();
            if (string.IsNullOrEmpty(privateKeyBase64))
            {
                throw new InvalidOperationException("RSA private key is not available.");
            }
            return Convert.FromBase64String(privateKeyBase64);
        }
    }
}

