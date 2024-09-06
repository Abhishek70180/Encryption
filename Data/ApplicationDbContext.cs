using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test3enc.Models;

namespace Test3enc.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EncryptedFile> EncryptedFiles { get; set; }
        public DbSet<EncryptionKey> EncryptionKeys { get; set; }  
    }
}
