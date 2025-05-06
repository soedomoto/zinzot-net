using Minio;
using Minio.DataModel.Args;

namespace ZinzotNet.Services
{
    public class S3Service : IS3Service
    {
        private readonly IMinioClient _minio;
        public string BucketName { get; } = "zinzot-media";

        public S3Service()
        {
            // _s3Client = new AmazonS3Client(
            //     configuration["AWS:AccessKey"],
            //     configuration["AWS:SecretKey"],
            //     s3Config
            // );

            _minio = new MinioClient()
                .WithEndpoint("c1r7.va.idrivee2-46.com")
                .WithRegion("Virginia")
                .WithCredentials("dWfg8RaqdJcooeT6nB7B", "Sa5MI4LcQby4m3PYQEcQT1VrIaNsj8PximnzdTlb")
                .WithSSL(true) // false if HTTP
                .Build();
        }

        // public async Task<string> UploadFileAsync(string key, Stream fileStream)
        // {
        //     try
        //     {
        //         var putRequest = new PutObjectRequest
        //         {
        //             BucketName = BucketName,
        //             Key = key,
        //             InputStream = fileStream,
        //             ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        //         };

        //         var response = await _s3Client.PutObjectAsync(putRequest);
        //         return $"https://{BucketName}.s3.amazonaws.com/{key}";
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error uploading to S3: {ex.Message}");
        //     }
        // }

        // public async Task<Stream> DownloadFileAsync(string key)
        // {
        //     try
        //     {
        //         var getRequest = new GetObjectRequest
        //         {
        //             BucketName = BucketName,
        //             Key = key
        //         };

        //         var response = await _s3Client.GetObjectAsync(getRequest);
        //         return response.ResponseStream;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Error downloading from S3: {ex.Message}");
        //     }
        // }

        public async Task<string> GetPresignedUrl(string key)
        {
            try
            {
                // var request = new GetPreSignedUrlRequest
                // {
                //     BucketName = BucketName,
                //     Key = key,
                //     Expires = DateTime.UtcNow.AddHours(1),
                //     ContentType = "application/pdf",
                // };

                // return _s3Client.GetPreSignedURL(request);

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