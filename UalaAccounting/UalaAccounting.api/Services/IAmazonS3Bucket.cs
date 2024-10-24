using System;
namespace UalaAccounting.api.Services
{
	public interface IAmazonS3Bucket
	{
        void SetAmazonApiSecret(string amazonApiSecret);
        void SetAmazonApiKey(string amazonApiKey);
        void SetS3BucketName(string s3BucketName);
        Task<bool> UploadFileToS3(string fileSource, string path);
    }
}

