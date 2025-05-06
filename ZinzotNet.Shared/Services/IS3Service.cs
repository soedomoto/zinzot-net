namespace ZinzotNet.Services
{
    public interface IS3Service
    {
        public string BucketName { get; }
        Task<string> GetPresignedUrl(string key);
    }
}