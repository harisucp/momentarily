using System.IO;
using ImageResizer;
using Apeek.Common.Models;
namespace Apeek.Core.Services.Impl.Img
{
    public class ResizeImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            //Let the image builder add the correct extension based on the output file type
            bool disposeStream = false;
            if (imageHandlerTarget.Stream == null || !imageHandlerTarget.Stream.CanWrite)
            {
                imageHandlerTarget.Stream = new MemoryStream();
                disposeStream = true;
            }
            ImageBuilder.Current.Build(imageHandlerTarget.Bytes, imageHandlerTarget.Stream, new ResizeSettings(imageHandlerTarget.UpdateParams.ToString()), false, true);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
            if(disposeStream)
                imageHandlerTarget.Stream.Close();
        }
    }
}