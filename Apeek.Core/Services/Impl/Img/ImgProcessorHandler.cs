using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public abstract class ImgProcessorHandler
    {
        protected ImgProcessorHandler _nextImgProcessorHandler;
        public ImgProcessorHandler SetNext(ImgProcessorHandler nextImgProcessorHandler)
        {
            _nextImgProcessorHandler = nextImgProcessorHandler;
            return _nextImgProcessorHandler;
        }
        public abstract void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget);
    }
}