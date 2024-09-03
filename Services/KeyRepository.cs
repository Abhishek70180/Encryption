using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test3enc.Data;
using Test3enc.Models;
using Test3enc.Services.IServices;

public class KeyRepository : IKeyRepository
{
    private readonly ApplicationDbContext _context;

    public KeyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetRsaPrivateKeyAsync()
    {
        var keyRecord = await _context.Keys.FirstOrDefaultAsync();
        if (keyRecord == null)
        {
            await GenerateAndStoreRsaKeyPairAsync();
            keyRecord = await _context.Keys.FirstOrDefaultAsync();
        }
        return keyRecord?.PrivateKeyBase64;
    }

    private async Task GenerateAndStoreRsaKeyPairAsync()
    {
        using (var rsa = RSA.Create())
        {
            rsa.KeySize = 2048;
            string privateKeyBase64 = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            string publicKeyBase64 = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            var keyRecord = new KeyRecord
            {
                PrivateKeyBase64 = privateKeyBase64,
                PublicKeyBase64 = publicKeyBase64
            };

            _context.Keys.Add(keyRecord);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GetRsaPublicKeyAsync()
    {
        var keyRecord = await _context.Keys.FirstOrDefaultAsync();
        if (keyRecord == null)
        {
            await GenerateAndStoreRsaKeyPairAsync();
            keyRecord = await _context.Keys.FirstOrDefaultAsync();
        }
        return keyRecord?.PublicKeyBase64;
    }
}
