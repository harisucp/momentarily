using Apeek.Common;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class DeleteGoodImages : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            Ioc.Get<IImageDataService>().DeleteGoodImg(imageHandlerTarget.UserId, imageHandlerTarget.GoodId.Value, imageHandlerTarget.Sequence);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}