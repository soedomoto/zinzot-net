using Minio;
using Minio.DataModel.Args;

namespace ZinzotNet.Services
{
    public class S3Service : IS3Service
    {
        private readonly IMinioClient _minio;
        public string BucketName { get; } = Constants.S3BucketName;

        public S3Service()
        {
            _minio = new MinioClient()
                .WithEndpoint(Constants.S3Endpoint)
                .WithRegion(Constants.S3Region)
                .WithCredentials(Constants.S3AccessKey, Constants.S3SecretKey)
                .WithSSL(true) 
                .Build();
        }

        public async Task<string> GetPresignedUrl(string key)
        {
            try
            {
                return await _minio.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(key)
                    .WithExpiry(60 * 60)
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating presigned URL: {ex.Message}");
            }
        }
    }
}