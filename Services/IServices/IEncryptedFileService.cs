using Test3enc.Models;

namespace Test3enc.Services.IServices
{
    public interface IEncryptedFileService
    {
        Task<IEnumerable<EncryptedFile>> GetAllEncryptedFilesAsync();
        Task<EncryptedFile> GetEncryptedFileByIdAsync(int id);
        Task SaveEncryptedFileAsync(EncryptedFile encryptedFile);
        Task DeleteEncryptedFileAsync(int id);
    }
}
