using Apeek.Common;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class GetOriginalUserImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            var userImg = Ioc.Get<IImageDataService>().GetUserImage(imageHandlerTarget.UserId, imageHandlerTarget.Sequence, imageHandlerTarget.Type);
            imageHandlerTarget.FileName = userImg.FileName;
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}