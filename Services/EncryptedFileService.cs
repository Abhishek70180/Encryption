using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test3enc.Data;
using Test3enc.Models;
using Test3enc.Services.IServices;

namespace Test3enc.Services
{
    public class EncryptedFileService : IEncryptedFileService
    {
        private readonly ApplicationDbContext _context;

        public EncryptedFileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EncryptedFile>> GetAllEncryptedFilesAsync()
        {
            return await _context.EncryptedFiles.ToListAsync();
        }

        public async Task<EncryptedFile> GetEncryptedFileByIdAsync(int id)
        {
            return await _context.EncryptedFiles.FindAsync(id);
        }

        public async Task SaveEncryptedFileAsync(EncryptedFile encryptedFile)
        {
            if (encryptedFile.Id == 0)
            {
                _context.EncryptedFiles.Add(encryptedFile);
            }
            else
            {
                _context.EncryptedFiles.Update(encryptedFile);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEncryptedFileAsync(int id)
        {
            var encryptedFile = await _context.EncryptedFiles.FindAsync(id);
            if (encryptedFile != null)
            {
                _context.EncryptedFiles.Remove(encryptedFile);
                await _context.SaveChangesAsync();
            }
        }
    }
}