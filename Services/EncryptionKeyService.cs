using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test3enc.Data;
using Test3enc.Models;
using Test3enc.Services.IServices;

namespace Test3enc.Services
{
    public class EncryptionKeyService : IEncryptionKeyService
    {
        private readonly ApplicationDbContext _context;

        public EncryptionKeyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EncryptionKey>> GetAllKeysAsync()
        {
            return await _context.EncryptionKeys.ToListAsync();
        }

        public async Task<EncryptionKey> GetKeyByNameAsync(string name)
        {
            return await _context.EncryptionKeys.FirstOrDefaultAsync(k => k.Name == name);
        }

        public async Task<EncryptionKey> GetKeyByIdAsync(int id)
        {
            return await _context.EncryptionKeys.FindAsync(id);
        }

        public async Task CreateKeyAsync(string name, byte[] key)
        {
            var encryptionKey = new EncryptionKey
            {
                Name = name,
                Key = key,
                CreatedAt = DateTime.UtcNow
            };
            _context.EncryptionKeys.Add(encryptionKey);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteKeyAsync(int id)
        {
            var encryptionKey = await _context.EncryptionKeys.FindAsync(id);
            if (encryptionKey != null)
            {
                _context.EncryptionKeys.Remove(encryptionKey);
                await _context.SaveChangesAsync();
            }
        }
    }
}