using System.Collections.Generic;
using Apeek.Common;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Entities.Extensions;
namespace Apeek.Core.Services.Impl.Img
{
    public class ReadGoodImagesFromDb : ImgProcessorHandler
    {
        private Dictionary<ImageType, ImageUpdateParams> _imageSizes;
        public ReadGoodImagesFromDb()
        {
            _imageSizes = new Dictionary<ImageType, ImageUpdateParams>
            {
                {ImageType.Thumb, new ImageUpdateParams() {width = "144", height = "108", bgcolor = ConstantsImage.DefaultImageBgColor}},
                {ImageType.Normal, new ImageUpdateParams() {width = "612", height = "459"}},
                {ImageType.Large, new ImageUpdateParams() {width = "1152", height = "864"}}
            };
        }
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            //if images exists we have to use their names otherwise we will generate new names
            imageHandlerTarget.Images = Ioc.Get<IImageDataService>().GetGoodImages(imageHandlerTarget.UserId, imageHandlerTarget.GoodId.Value, imageHandlerTarget.Sequence).ConvertAll(x => (IUserImg)x);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
    }
}