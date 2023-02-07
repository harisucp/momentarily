using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Common;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class ReadImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            var fileStorageService = Ioc.Get<IExternalFileStorageService>();
            imageHandlerTarget.Bytes = fileStorageService.ReadFile(imageHandlerTarget.ImageFolder.ToString(), imageHandlerTarget.FileName);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}