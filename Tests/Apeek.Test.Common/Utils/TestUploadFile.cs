using System.IO;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Core.Services.Impl.AWS.S3;
namespace Apeek.Test.Common.Utils
{
    public class TestUploadFile
    {
        public static void WriteFile(string fileName)
        {
            string filePath = AppSettings.GetInstance().AppdataDirectory + fileName;
            var s3 = Ioc.Get<IExternalFileStorageService>();
            s3.WriteFile(new StreamReader(filePath).BaseStream, fileName);
        }
        public static void DeleteFile(string fileName)
        {
            var s3 = Ioc.Get<IExternalFileStorageService>();
            s3.DeleteFile(fileName);
        }
        public static bool Exists(string fileName)
        {
            var s3 = Ioc.Get<IExternalFileStorageService>();
            return s3.Exists(fileName);
        } 
    }
}