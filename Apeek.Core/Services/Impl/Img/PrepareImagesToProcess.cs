using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Apeek.Core.Services.Impl.Img
{
    public class PrepareImagesToProcess : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            foreach (var suffix in imageHandlerTarget.DefaultImageSizes.Keys)
            {
                if (imageHandlerTarget.UpdateParams == null)
                    imageHandlerTarget.UpdateParams = new ImageUpdateParams();
                var targetToProcess = new ImgProcessorHandlerTarget
                {
                    Type = (int)suffix,
                    UserId = imageHandlerTarget.UserId,
                    GoodId = imageHandlerTarget.GoodId,
                    Sequence = imageHandlerTarget.Sequence,
                    UpdateParams = imageHandlerTarget.UpdateParams.MergeAndCreateNew(imageHandlerTarget.DefaultImageSizes[suffix]),
                    Bytes = imageHandlerTarget.Bytes,
                    ImageFolder = imageHandlerTarget.ImageFolder
                };
                var image = imageHandlerTarget.Images.Find(x => x.Type == (int)suffix);
                if (image == null)
                    targetToProcess.FileName = ImageProcessor.GetImageName(imageHandlerTarget.UserId, imageHandlerTarget.GoodId, suffix, string.Format(".{0}", ConstantsImage.DefaultImageExt));
                else
                    targetToProcess.FileName = image.FileName;
                if (_nextImgProcessorHandler != null)
                    _nextImgProcessorHandler.HandleImage(targetToProcess);
            }
        }
    }
}