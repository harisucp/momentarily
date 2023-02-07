namespace Apeek.Core.Services.Impl.AWS.S3
{
    public interface IExternalFileStorageConfig
    {
        string BucketName { get; set; }
        string AccessKey { get; set; }
        string SecretKey { get; set; }
        int PreSignedUrlTimeout { get; set; }
    }
    public class ExternalFileStorageConfig : IExternalFileStorageConfig
    {
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public int PreSignedUrlTimeout { get; set; }
    }
}