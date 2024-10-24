using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace UalaAccounting.api.Services
{
	public class AmazonS3Bucket : IAmazonS3Bucket
    {
        private readonly ILogger<AmazonS3Bucket> _logger;
        private string _amazonApiSecret = "";
        private string _amazonApiKey = "";
        private string _s3BucketName = "";

        public AmazonS3Bucket(ILogger<AmazonS3Bucket> logger)
        {
            _logger = logger;
        }

        public void SetAmazonApiSecret(string amazonApiSecret)
        {
            this._amazonApiSecret = amazonApiSecret;
        }

        public void SetAmazonApiKey(string amazonApiKey)
        {
            this._amazonApiKey = amazonApiKey;
        }

        public void SetS3BucketName(string s3BucketName)
        {
            this._s3BucketName = s3BucketName;
        }

        public async Task<bool> UploadFileToS3(string fileSource, string destPath)
        {


            PutObjectRequest putRequest;
            PutObjectResponse response;
            IAmazonS3 client;

            try
            {

                //TODO: send the region as a parameter or configuration
                client = new AmazonS3Client(RegionEndpoint.USEast1);

                putRequest = new PutObjectRequest
                {
                    FilePath = fileSource,
                    BucketName = this._s3BucketName,
                    Key = destPath
                };

                response = await client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Successfully uploaded {fileSource} to {this._s3BucketName}({System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    _logger.LogError($"AmazonS3Bucket PerformCopyingFileToS3(): {amazonS3Exception}");

                    throw amazonS3Exception;
                }
            }
        }

    }
}

