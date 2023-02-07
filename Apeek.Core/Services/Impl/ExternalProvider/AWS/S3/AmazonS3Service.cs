using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Apeek.Common;
using Apeek.Common.Converters;
using Apeek.Common.Extensions;
using Apeek.Common.Logger;
using Apeek.Core.Services.Impl.AWS.S3;
namespace Apeek.Core.Services.Impl.ExternalProvider.AWS.S3
{
    public class AmazonS3Service : IExternalFileStorageService
    {
        private readonly IExternalFileStorageConfig _externalFileStorageConfig;
        public AmazonS3Service(IExternalFileStorageConfig externalFileStorageConfig)
        {
            _externalFileStorageConfig = externalFileStorageConfig;
        }
        public string GetWriteUrl(string fileName)
        {
            return GeneratePreSignedUrl(fileName);
        }
        IAmazonS3 GetAmazonS3Client()
        {
            AWSConfigs.S3Config.UseSignatureVersion4 = true;
            var credentials = new BasicAWSCredentials(_externalFileStorageConfig.AccessKey, _externalFileStorageConfig.SecretKey);
            return new AmazonS3Client(credentials, RegionEndpoint.EUCentral1);
        }
        private string GeneratePreSignedUrl(string fileName)
        {
            try
            {
                using (IAmazonS3 s3Client = GetAmazonS3Client())
                {
                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                    {
                        BucketName = _externalFileStorageConfig.BucketName,
                        Key = fileName,
                        Verb = HttpVerb.PUT,
                        Expires = DateTime.Now.AddMinutes(_externalFileStorageConfig.PreSignedUrlTimeout),
                    };
                    return s3Client.GetPreSignedURL(request);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, string.Format("Cannot generate presigned url, error: Check the provided AWS Credentials. To sign up for service, go to http://aws.amazon.com/s3"));
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, string.Format("Cannot generate presigned url, exception: {0}", amazonS3Exception));
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, string.Format("Cannot generate presigned url, exception: {0}", ex));
            }
            return null;
        }
        public void DeleteFile(string name)
        {
            DeleteFiles(new List<string>() { name });
        }
        public void DeleteFiles(List<string> names)
        {
            try
            {
                if (names != null && names.Any())
                {
                    using (IAmazonS3 s3Client = GetAmazonS3Client())
                    {
                        foreach (var name in names)
                        {
                            if (string.IsNullOrWhiteSpace(name))
                                continue;
                            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = _externalFileStorageConfig.BucketName,
                                Key = name
                            };
                            s3Client.DeleteObject(deleteObjectRequest);
                        }
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, "Check the provided AWS Credentials.");
                    Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, "For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.AmazonS3Service, string.Format("Error occurred. Message:'{0}' when deleting an objects {1}", amazonS3Exception.Message, CustomConverters.ListToSeparatedString(names)));
                }
            }
        }
        public byte[] ReadFile(string fileName)
        {
            byte [] responseBody;
            using (IAmazonS3 s3Client = GetAmazonS3Client())
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _externalFileStorageConfig.BucketName,
                    Key = fileName
                };
                using (GetObjectResponse response = s3Client.GetObject(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    responseBody = responseStream.ReadToEnd();
                }
            }
            return responseBody;
        }
        public void WriteFile(Stream inputStream, string fileName)
        {
            try
            {
                using (IAmazonS3 s3Client = GetAmazonS3Client())
                {
                    var putRequest2 = new PutObjectRequest
                    {
                        BucketName = _externalFileStorageConfig.BucketName,
                        Key = fileName,
                        CannedACL = S3CannedACL.PublicRead,
                        InputStream = inputStream,
                        ContentType = "text/plain"
                    };
                    putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
                    var response2 = s3Client.PutObject(putRequest2);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine("For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("Error occurred. Message:'{0}' when writing an object", amazonS3Exception.Message);
                }
            }
        }
        public bool Exists(string fileName)
        {
            try
            {
                using (IAmazonS3 s3Client = GetAmazonS3Client())
                {
                    var request = new GetObjectMetadataRequest()
                    {
                        BucketName = _externalFileStorageConfig.BucketName,
                        Key = fileName
                    };
                    var response = s3Client.GetObjectMetadata(request);
                }
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return false;
                //status wasn't not found, so throw the exception
                throw;
            }
        }
        public void WriteFile(byte[] bytes, string imgFolder, string fileName)
        {
            WriteFile(bytes, string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void WriteFile(Stream inputStream, string imgFolder, string fileName)
        {
            WriteFile(inputStream, string.Format("{0}/{1}", imgFolder, fileName));
        }
        public byte[] ReadFile(string imgFolder, string fileName)
        {
            return ReadFile(string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void DeleteFile(string imgFolder, string fileName)
        {
            DeleteFile(string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void WriteFileToUrl(byte[] bytes, string url)
        {
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                dataStream.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
        }
        public void WriteFile(byte [] bytes, string fileName)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                WriteFile(stream, fileName);
            }
        }
    }
}