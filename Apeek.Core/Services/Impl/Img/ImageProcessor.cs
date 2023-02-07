using System;
using System.Text;
using Apeek.Core.Services.Impl.MSMQ;
using Apeek.Common;
using Apeek.Common.EventManager.DataTracker;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Validators;
namespace Apeek.Core.Services.Impl.Img
{
    public class ImageProcessor : MsmqProcessorPlugin, IDependency
    {
        public override string PluginName { get { return MsmqPluginName.ImageProcessor; } }
        protected override int ProcessEnvelop(MsmqEnvelope envelope)
        {
            var result = Process((ImgProcessorHandlerTarget) envelope.MsmqMessage);
            Answer = null;
            return result;
        }
        public int Process(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if (imgProcessorHandlerTarget == null || !ValidatorBase.Identity(imgProcessorHandlerTarget.UserId))
            {
                var message = string.Format("Cannot process image: {0}", imgProcessorHandlerTarget);
                Ioc.Get<IDbLogger>().LogError(LogSource.ImageProcessor, message);
                return ProcessStatus.Error;
            }
            try
            {
                return ProcessorByOperationType(imgProcessorHandlerTarget);
            }
            catch (Exception ex)
            {
                var message = string.Format("Cannot process image: {0}", imgProcessorHandlerTarget);
                Ioc.Get<IDbLogger>().LogException(LogSource.ImageProcessor, message, ex);
            }
            return ProcessStatus.Error;
        }
        private int ProcessorByOperationType(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if (imgProcessorHandlerTarget.OperationType == OperationType.InsertOriginal)
            {
                if (string.IsNullOrWhiteSpace(imgProcessorHandlerTarget.FileName))
                    return ProcessStatus.Error;
                ImgProcessorHandler startImgProcessorHandler = new RotateImage();
                startImgProcessorHandler
                    .SetNext(new WriteImage());
                startImgProcessorHandler.HandleImage(imgProcessorHandlerTarget);
            }
            else if (imgProcessorHandlerTarget.OperationType == OperationType.Insert)
            {
                if (string.IsNullOrWhiteSpace(imgProcessorHandlerTarget.FileName))
                    return ProcessStatus.Error;
                ImgProcessorHandler startImgProcessorHandler = new ReadImage();
                startImgProcessorHandler
                    .SetNext(GetReadImageFromDb(imgProcessorHandlerTarget))
                    .SetNext(new PrepareImagesToProcess())
                    .SetNext(new ResizeImage())
                    .SetNext(new WriteImage())
                    .SetNext(GetUpdateImgHandler(imgProcessorHandlerTarget));
                //process and create different image formats
                startImgProcessorHandler.HandleImage(imgProcessorHandlerTarget);
                //process original user image
                GetUpdateImgHandler(imgProcessorHandlerTarget).HandleImage(imgProcessorHandlerTarget);
            }
            else if (imgProcessorHandlerTarget.OperationType == OperationType.Update)
            {
                ImgProcessorHandler startImgProcessorHandler = GetOriginalImageHandler(imgProcessorHandlerTarget);
                startImgProcessorHandler.SetNext(new ReadImage())
                    .SetNext(GetReadImageFromDb(imgProcessorHandlerTarget))
                    .SetNext(new PrepareImagesToProcess())
                    .SetNext(new ResizeImage())
                    .SetNext(new WriteImage());
                startImgProcessorHandler.HandleImage(imgProcessorHandlerTarget);
                //process original user image
                var processorForOriginalImg = new ResizeImage();
                processorForOriginalImg.SetNext(new WriteImage());
                processorForOriginalImg.HandleImage(imgProcessorHandlerTarget);
            }
            else if (imgProcessorHandlerTarget.OperationType == OperationType.Delete)
            {
                ImgProcessorHandler startImgProcessorHandler = GetDeleteImagesHandler(imgProcessorHandlerTarget);
                startImgProcessorHandler.HandleImage(imgProcessorHandlerTarget);
            }
            return ProcessStatus.Processed;
        }
        private ImgProcessorHandler GetReadImageFromDb(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if (imgProcessorHandlerTarget.GoodId.HasValue)
                return new ReadGoodImagesFromDb();
            return new ReadUserImagesFromDb();
        }
        private ImgProcessorHandler GetUpdateImgHandler(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if(imgProcessorHandlerTarget.GoodId.HasValue)
                return new UpdateGoodImage();
            return new UpdateUserImage();
        }
        private ImgProcessorHandler GetOriginalImageHandler(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if(imgProcessorHandlerTarget.GoodId.HasValue)
                return new GetOriginalGoodImage();
            return new GetOriginalUserImage();
        }
        private ImgProcessorHandler GetDeleteImagesHandler(ImgProcessorHandlerTarget imgProcessorHandlerTarget)
        {
            if(imgProcessorHandlerTarget.GoodId.HasValue)
                return new DeleteGoodImages();
            return new DeleteUserImages();
        }
        public static string GetImageName(int userId, int? serviceId, ImageType suffix, string fileExtension)
        {
            var prefix = new StringBuilder();
            prefix.Append(userId);
            if (serviceId.HasValue)
            {
                prefix.Append(string.Format("-{0}",serviceId));
            }
            var imgName = string.Format("{0}-{1}-{2}{3}", prefix, ShortGuid.NewGuid(), suffix, fileExtension.ToLower());
            return imgName;
        }
    }
}