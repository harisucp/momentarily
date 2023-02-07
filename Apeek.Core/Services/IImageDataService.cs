using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services
{
    public interface IImageDataService : IDependency
    {
        bool DeleteUserImages(int userId);
        void DeleteUserImages(int userId, int sequence);
        void DeleteGoodImg(int userId, int serviceId, int sequence);
        void DeleteGoodImg(int userId, int serviceId, IUnitOfWork unitOfWork = null);
        List<UserImg> GetUserImages(int userId, ImageType imageType);
        List<UserImg> GetUserImages(int userId, int sequence, IUnitOfWork unitOfWork = null);
        UserImg GetUserImage(int userId, int sequence, int imageType, IUnitOfWork unitOfWork = null);
        List<UserImg> GetUserImages(int userId, IUnitOfWork unitOfWork = null);
        List<GoodImg> GetGoodImages(int userId, int serviceId, IUnitOfWork unitOfWork = null);
        List<GoodImg> GetGoodImages(int userId, int serviceId, int sequence, IUnitOfWork unitOfWork = null);
        GoodImg GetGoodImage(int userId, int? serviceId, int sequence, int imageType, IUnitOfWork unitOfWork = null);
        List<GoodImg> GetGoodImagesByType(int userId, int serviceId, int imageType, IUnitOfWork unitOfWork = null);
        UserImageModel InsertOriginalUserImage(UserImageModel imageModel);
        UserImageModel InsertOriginalGoodImage(UserImageModel imageModel);
        Result<UserImageModel> RefreshUserImage(RefreshUserImageModel model);
        Result<UserImageModel> RefreshGoodImage(RefreshUserImageModel model);
        bool DeleteUserImage(UserImg userImg);
        bool DeleteGoodImage(GoodImg userImg);
        bool DeleteGoodImages(int userId, int imageId);
        bool DeleteImages(int userId, int imageId, ImageFolder imageFolder);
        void RefreshGoodImageSequence(UserImageModel model);
    }
}