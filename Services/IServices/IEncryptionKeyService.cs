using System.Collections.Generic;
using System.Threading.Tasks;
using Test3enc.Models;

namespace Test3enc.Services.IServices
{
    public interface IEncryptionKeyService
    {
        Task<IEnumerable<EncryptionKey>> GetAllKeysAsync();
        Task<EncryptionKey> GetKeyByNameAsync(string name);
        Task<EncryptionKey> GetKeyByIdAsync(int id);
        Task CreateKeyAsync(string name, byte[] key);
        Task DeleteKeyAsync(int id);
    }
}