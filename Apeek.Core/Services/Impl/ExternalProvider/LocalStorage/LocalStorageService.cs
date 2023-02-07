using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Core.Services.Impl.AWS.S3;
namespace Apeek.Core.Services.Impl.ExternalProvider.LocalStorage
{
    public class LocalStorageService : IExternalFileStorageService
    {
        public string GetWriteUrl(string fileName)
        {
            try
            {
                return HttpContextFactory.Current.Server.MapPath(
                    AppSettings.GetInstance().ImageLocalStoragePath + fileName);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogError(LogSource.LocalStorageService, string.Format("Get write url to file in loca storage fail. Exception: {0}", ex));
            }
            return null;
        }
        public void DeleteFile(string imgFolder, string fileName)
        {
            DeleteFile(string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void DeleteFile(string fileName)
        {
            DeleteFiles(new List<string>() { fileName });
        }
        public void DeleteFiles(List<string> names)
        {
            try
            {
                foreach (var fileName in names)
                {
                    var filePath = HttpContextFactory.Current.Server.MapPath(
                        AppSettings.GetInstance().ImageLocalStoragePath + fileName);
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogError(LogSource.LocalStorageService, string.Format("Delete file from local storage fail. Exception: {0}", ex));
            }
        }
        public byte[] ReadFile(string imgFolder, string fileName)
        {
            return ReadFile(string.Format("{0}/{1}", imgFolder, fileName));
        }
        public byte[] ReadFile(string fileName)
        {
            try
            {
                var filePath = HttpContextFactory.Current.Server.MapPath(
                    AppSettings.GetInstance().ImageLocalStoragePath + fileName);
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogError(LogSource.LocalStorageService, string.Format("Read file from local storage fail. Exception: {0}", ex));
            }
            return null;
        }
        public void WriteFile(byte[] bytes, string imgFolder, string fileName)
        {
            WriteFile(bytes, string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void WriteFile(byte[] bytes, string fileName)
        {
            using (var fileStream = new MemoryStream(bytes))
            {
                WriteFile(fileStream, fileName);
            }
        }
        public void WriteFile(Stream inputStream, string imgFolder, string fileName)
        {
            WriteFile(inputStream, string.Format("{0}/{1}", imgFolder, fileName));
        }
        public void WriteFile(Stream inputStream, string fileName)
        {
            try
            {
                var filePath = HttpContextFactory.Current.Server.MapPath(
                    AppSettings.GetInstance().ImageLocalStoragePath + fileName);
                var directoryPath = Path.GetDirectoryName(filePath);
                if (directoryPath != null && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (var fileStream = File.Create(filePath))
                {
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogError(LogSource.LocalStorageService, string.Format("Upload file to local storage failed. Exception: {0}", ex));
            }
        }
        public bool Exists(string fileName)
        {
            try
            {
                var filePath = HttpContextFactory.Current.Server.MapPath(
                    AppSettings.GetInstance().ImageLocalStoragePath + fileName);
                if (File.Exists(filePath)) return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogError(LogSource.LocalStorageService, string.Format("File not exists in local storage. Exception: {0}", ex));
            }
            return false;
        }

        public void WriteFileToUrl(byte[] bytes, string url)
        {
            throw new NotImplementedException();
        }
    }
}
