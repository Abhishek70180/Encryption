using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Test3enc.Models;
using Test3enc.Services.IServices;
using Test3enc.Models.ViewModel;
using System.Security.Cryptography;

namespace Test3enc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAesEncryptionService _aesEncryptionService;
        private readonly ITripleDesEncryptionService _tripleDesEncryptionService;
        private readonly IRsaEncryptionService _rsaEncryptionService;
        private readonly IEncryptedFileService _encryptedFileService;

        public HomeController(
            IAesEncryptionService aesEncryptionService,
            ITripleDesEncryptionService tripleDesEncryptionService,
            IRsaEncryptionService rsaEncryptionService,
            IEncryptedFileService encryptedFileService)
        {
            _aesEncryptionService = aesEncryptionService;
            _tripleDesEncryptionService = tripleDesEncryptionService;
            _rsaEncryptionService = rsaEncryptionService;
            _encryptedFileService = encryptedFileService;
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
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("Index");
            }

            EncryptedFile encryptedFile;

            try
            {
                switch (algorithm)
                {
                    case "AES":
                        var aesKey = GenerateAesKey();
                        encryptedFile = await _aesEncryptionService.EncryptFileAsync(file, aesKey);
                        break;
                    case "TripleDES":
                        var tripleDesKey = GenerateTripleDesKey();
                        encryptedFile = await _tripleDesEncryptionService.EncryptFileAsync(file, tripleDesKey);
                        break;
                    case "RSA":
                        var rsaKeys = _rsaEncryptionService.GenerateRsaKeyPair();
                        encryptedFile = await _rsaEncryptionService.EncryptFileAsync(file, rsaKeys.PublicKey);
                        encryptedFile.PrivateKey = rsaKeys.PrivateKey;
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
                    case "TripleDES":
                        decryptedData = await _tripleDesEncryptionService.DecryptFileAsync(encryptedFile);
                        break;
                    case "RSA":
                        decryptedData = await _rsaEncryptionService.DecryptFileAsync(encryptedFile, encryptedFile.PrivateKey);
                        break;
                    default:
                        throw new ArgumentException("Invalid encryption algorithm");
                }

                await _encryptedFileService.DeleteEncryptedFileAsync(id);
                string base64String = Convert.ToBase64String(decryptedData);
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

        private byte[] GenerateTripleDesKey()
        {
            using (var tripleDes = TripleDES.Create())
            {
                tripleDes.KeySize = 192;
                tripleDes.GenerateKey();
                return tripleDes.Key;
            }
        }
    }
}
