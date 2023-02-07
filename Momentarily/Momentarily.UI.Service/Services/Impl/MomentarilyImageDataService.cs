using Apeek.Core.Services.Impl;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Core.Services.Impl.Img;
namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyImageDataService : ImageDataService, IMomentarilyImageDataService
    {
        public MomentarilyImageDataService(IExternalFileStorageService externalFileStorage, ImageProcessor imageProcessor)
            : base(externalFileStorage, imageProcessor)
        {
        }
    }
}
