using Apeek.Common;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class DeleteUserImages : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            Ioc.Get<IImageDataService>().DeleteUserImages(imageHandlerTarget.UserId, imageHandlerTarget.Sequence);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}