using System.Collections.Generic;
using System.IO;
namespace Apeek.Core.Services.Impl.AWS.S3
{
    public interface IExternalFileStorageService
    {
        string GetWriteUrl(string fileName);
        void DeleteFile(string fileName);
        void DeleteFiles(List<string> names);
        byte [] ReadFile(string fileName);
        void WriteFile(byte[] bytes, string fileName);
        void WriteFileToUrl(byte[] bytes, string url);
        void WriteFile(Stream inputStream, string fileName);
        bool Exists(string fileName);
        void WriteFile(byte[] bytes, string imgFolder, string fileName);
        void WriteFile(Stream inputStream, string imgFolder, string fileName);
        byte[] ReadFile(string imgFolder, string fileName);
        void DeleteFile(string imgFolder, string fileName);
    }
}