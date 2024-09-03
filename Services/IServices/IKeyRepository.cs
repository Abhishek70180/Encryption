namespace Test3enc.Services.IServices
{
    public interface IKeyRepository
    {
        public Task<string> GetRsaPrivateKeyAsync();
        public Task<string> GetRsaPublicKeyAsync();
    }
}
