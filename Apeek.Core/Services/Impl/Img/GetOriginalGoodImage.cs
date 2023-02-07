using Apeek.Common;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class GetOriginalGoodImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            var userServiceImg = Ioc.Get<IImageDataService>().GetGoodImage(imageHandlerTarget.UserId, imageHandlerTarget.GoodId.Value, imageHandlerTarget.Sequence, imageHandlerTarget.Type);
            imageHandlerTarget.FileName = userServiceImg.FileName;
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}