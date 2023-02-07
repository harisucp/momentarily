using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class UserImageMapper : IUserImageMapper
    {
        public UserImageModel Map(UserImg source, UserImageModel dest)
        {
            if(source == null)
                return dest;
            dest.Id = source.Id;
            dest.UserId = source.UserId;
            dest.FileName = source.FileName;
            dest.ImgFolder = ImageFolder.User;
            return dest;
        }
    }
}
