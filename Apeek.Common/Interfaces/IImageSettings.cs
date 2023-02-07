using System.Collections.Generic;
using Apeek.Entities.Constants;
using Apeek.Entities.Extensions;
namespace Apeek.Common.Interfaces
{
    public interface IImageSettings : IDependency
    {
        List<string> ImageExtensions { get; }
        Dictionary<ImageType, ImageUpdateParams> UserImageSizes { get; }
        Dictionary<ImageType, ImageUpdateParams> GoodImageSizes { get; }
    }
}