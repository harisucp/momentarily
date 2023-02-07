using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Apeek.Common;
using Apeek.Common.EventManager.DataTracker;
using Apeek.Common.Extensions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Core.Services.Impl.Img;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.ViewModels.Models;
using ExifLib;
namespace Apeek.Core.Services.Impl
{
    public class ImageDataService : IImageDataService
    {
        private readonly IExternalFileStorageService _externalFileStorage;
        private readonly ImageProcessor _imageProcessor;
        public ImageDataService(IExternalFileStorageService externalFileStorage, ImageProcessor imageProcessor)
        {
            _externalFileStorage = externalFileStorage;
            _imageProcessor = imageProcessor;
        }
        private bool InsertImage(UserImageModel imageModel)
        {
            try
            {
                ImgProcessorHandlerTarget target = new ImgProcessorHandlerTarget
                {
                    UserId = imageModel.UserId,
                    FileName = imageModel.FileName,
                    GoodId = imageModel.GoodId,
                    OperationType = OperationType.InsertOriginal,
                    ImageFolder = imageModel.ImgFolder,
                    DefaultImageSizes = imageModel.ImgSettings,
                    Sequence = imageModel.Sequence,
                    Stream = imageModel.InputStream
                };
                if (ProcessImage(target))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Process image fail. Ex: {0}.", ex));
            }
            return false;
        }
        public UserImageModel InsertOriginalUserImage(UserImageModel imageModel)
        {
            try
            {
                if (InsertImage(imageModel))
                {
                    UserImageModel userImageModel = null;
                    Uow.Wrap(u =>
                    {
                        var userImage = new UserImg
                        {
                            UserId = imageModel.UserId,
                            FileName = imageModel.FileName,
                            Type = (int) ImageType.Original
                        };
                        var imageFromDb = new Repository<UserImg>().Save(userImage);
                        userImageModel = new UserImageModel()
                        {
                            UserId = imageFromDb.UserId,
                            Id = imageFromDb.Id,
                            FileName = imageFromDb.FileName,
                            Url = imageFromDb.ImageUrl(ImageFolder.User.ToString())
                        };
                    },
                        null,
                        LogSource.PersonService);
                    return userImageModel;
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Fail to upload user image. Ex: {0}.", ex));
            }
            return null;
        }
        public UserImageModel InsertOriginalGoodImage(UserImageModel imageModel)
        {
            try
            {
                if (InsertImage(imageModel))
                {
                    UserImageModel userImageModel = null;
                    Uow.Wrap(u =>
                    {
                        var userServiceImage = new GoodImg
                        {
                            UserId = imageModel.UserId,
                            GoodId = imageModel.GoodId,
                            FileName = imageModel.FileName,
                            Type = (int) ImageType.Original,
                            Folder = imageModel.ImgFolder.ToString()
                        };
                        var imageFromDb = new Repository<GoodImg>().Save(userServiceImage);
                        userImageModel = new UserImageModel()
                        {
                            Id = imageFromDb.Id,
                            FileName = imageFromDb.FileName,
                            Url = imageFromDb.ImageUrl(imageModel.ImgFolder.ToString())
                        };
                    },
                        null,
                        LogSource.PersonService);
                    return userImageModel;
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Fail to upload user image. Ex: {0}.", ex));
            }
            return null;
        }
        public Result<UserImageModel> RefreshUserImage(RefreshUserImageModel model)
        {
            var result = new Result<UserImageModel>(CreateResult.Error, new UserImageModel());
            try
            {
                ImgProcessorHandlerTarget target = new ImgProcessorHandlerTarget
                {
                    UserId = model.UserId,
                    FileName = model.FileName,
                    OperationType = OperationType.Insert,
                    ImageFolder = ImageFolder.User,
                    DefaultImageSizes = model.ImgSettings
                };
                if (ProcessImage(target))
                    result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Process image fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool DeleteUserImage(UserImg userImg)
        {
            try
            {
                DeleteUserImages(new List<UserImg>()
                {
                    userImg
                });
                return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Delete image fail. Ex: {0}.", ex));
            }
            return false;
        }
        public Result<UserImageModel> RefreshGoodImage(RefreshUserImageModel model)
        {
            var result = new Result<UserImageModel>(CreateResult.Error, new UserImageModel());
            try
            {
                ImgProcessorHandlerTarget target = new ImgProcessorHandlerTarget
                {
                    UserId = model.UserId,
                    FileName = model.FileName,
                    GoodId = model.GoodId,
                    OperationType = OperationType.Insert,
                    ImageFolder = model.ImgFolder,
                    DefaultImageSizes = model.ImgSettings,
                    Sequence = model.Sequence
                };
                if (ProcessImage(target))
                    result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Process image fail. Ex: {0}.", ex));
            }
            return result;
        }
        public void RefreshGoodImageSequence(UserImageModel model)
        {
            try
            {
                Uow.Wrap(u =>
                {
                    var goodImageRep = new Repository<GoodImg>();
                    var image = goodImageRep.Get(model.Id);
                    if (image != null)
                    {
                        var currSequence = image.Sequence;
                        var goodImages = GetGoodImages(model.UserId, model.GoodId.Value, currSequence, u);
                        if (goodImages.Any())
                        {
                            goodImages.ForEach(p => p.Sequence = model.Sequence);
                            goodImageRep.SaveOrUpdate(goodImages);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Refresh good image sequence fail. Ex: {0}.", ex));
            }
        }
        public bool DeleteGoodImage(GoodImg userImg)
        {
            try
            {
                DeleteGoodImg(new List<GoodImg>()
                {
                    userImg
                });
                return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Delete image fail. Ex: {0}.", ex));
            }
            return false;
        }
        private bool ProcessImage(ImgProcessorHandlerTarget target)
        {
            try
            {
                if (_imageProcessor.Process(target) == ProcessStatus.Processed)
                    return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Process image fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool DeleteImages(int userId, int imageId, ImageFolder imageFolder)
        {
            try
            {
                switch (imageFolder)
                {
                    case ImageFolder.User:
                    {
                        return DeleteUserImages(userId);
                    }
                    case ImageFolder.Good:
                    {
                        return DeleteGoodImages(userId, imageId);
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Delete image fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool DeleteGoodImages(int userId, int imageId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var goodImageRep = new Repository<GoodImg>();
                    var goodImage = goodImageRep.Get(imageId);
                    if (goodImage != null && goodImage.UserId == userId)
                    {
                        if (goodImage.GoodId.HasValue)
                        {
                            var goodImages = GetGoodImages(goodImage.UserId, goodImage.GoodId.Value, goodImage.Sequence,
                                u);
                            if (goodImages.Any())
                                DeleteGoodImg(goodImages);
                        }
                        else
                        {
                            DeleteGoodImage(goodImage);
                        }
                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Delete good image fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool DeleteUserImages(int userId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var images = GetUserImages(userId, u);
                    if (images.Any())
                        DeleteUserImages(images);
                    result = true;
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Delete user image fail. Ex: {0}.", ex));
            }
            return result;
        }
        public void DeleteUserImages(int userId, int sequence)
        {
            Uow.Wrap(u =>
            {
                var images = GetUserImages(userId, sequence, u);
                if (images.Any())
                    DeleteUserImages(images);
            });
        }
        public void DeleteGoodImg(int userId, int serviceId, int sequence)
        {
            Uow.Wrap(u =>
            {
                var images = GetGoodImages(userId, serviceId, sequence, u);
                if (images.Any())
                    DeleteGoodImg(images);
            });
        }
        public void DeleteGoodImg(int userId, int serviceId, IUnitOfWork unitOfWork = null)
        {
            Uow.Wrap(u =>
            {
                var images = GetGoodImages(userId, serviceId, u);
                if (images.Any())
                    DeleteGoodImg(images);
            }, unitOfWork);
        }
        private void DeleteUserImages(List<UserImg> images)
        {
            var fileStorage = Ioc.Get<IExternalFileStorageService>();
            foreach (var img in images)
            {
                fileStorage.DeleteFile(ImageFolder.User.ToString(), img.FileName);
            }
            var rep = new Repository<UserImg>();
            rep.Delete(images);
        }
        private void DeleteGoodImg(List<GoodImg> images)
        {
            var fileStorage = Ioc.Get<IExternalFileStorageService>();
            foreach (var img in images)
            {
                fileStorage.DeleteFile(img.Folder, img.FileName);
            }
            var rep = new Repository<GoodImg>();
            rep.Delete(images);
        }
        public List<UserImg> GetUserImages(int userId, ImageType imageType)
        {
            var userImages = new List<UserImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<UserImg>().Table.Where(x => x.UserId == userId && x.Type == (int)imageType).ToList();
            }, null, LogSource.PersonService);
            return userImages;
        }
        public List<UserImg> GetUserImages(int userId, int sequence, IUnitOfWork unitOfWork = null)
        {
            var userImages = new List<UserImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<UserImg>().Table.Where(x => x.UserId == userId && x.Sequence == sequence).ToList();
            }, unitOfWork, LogSource.PersonService);
            return userImages;
        }
        public UserImg GetUserImage(int userId, int sequence, int imageType, IUnitOfWork unitOfWork = null)
        {
            UserImg userImg = null;
            Uow.Wrap(u =>
            {
                userImg = new Repository<UserImg>().Table.FirstOrDefault(x => x.UserId == userId && x.Sequence == sequence && x.Type == imageType);
            }, unitOfWork, LogSource.PersonService);
            return userImg;
        }
        public List<UserImg> GetUserImages(int userId, IUnitOfWork unitOfWork = null)
        {
            var userImages = new List<UserImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<UserImg>().Table.Where(x => x.UserId == userId).ToList();
            }, unitOfWork, LogSource.PersonService);
            return userImages;
        }
        public List<GoodImg> GetGoodImages(int userId, int serviceId, IUnitOfWork unitOfWork = null)
        {
            var userImages = new List<GoodImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<GoodImg>().Table.Where(x => x.UserId == userId && x.GoodId == serviceId).ToList();
            }, unitOfWork, LogSource.PersonService);
            return userImages;
        }
        public List<GoodImg> GetGoodImagesByType(int userId, int serviceId, int imageType, IUnitOfWork unitOfWork = null)
        {
            var userImages = new List<GoodImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<GoodImg>().Table.Where(x => x.UserId == userId && x.GoodId == serviceId && x.Type == imageType).ToList();
            }, unitOfWork, LogSource.PersonService);
            return userImages;
        }
        public List<GoodImg> GetGoodImages(int userId, int serviceId, int sequence, IUnitOfWork unitOfWork = null)
        {
            var userImages = new List<GoodImg>();
            Uow.Wrap(u =>
            {
                userImages = new Repository<GoodImg>().Table.Where(x => x.UserId == userId && x.GoodId == serviceId && x.Sequence == sequence).ToList();
            }, unitOfWork, LogSource.PersonService);
            return userImages;
        }
        public GoodImg GetGoodImage(int userId, int? serviceId, int sequence, int imageType, IUnitOfWork unitOfWork = null)
        {
            GoodImg goodImg = null;
            Uow.Wrap(u =>
            {
                goodImg = new Repository<GoodImg>().Table.FirstOrDefault(x => x.UserId == userId && x.GoodId == serviceId && x.Sequence == sequence && x.Type == imageType);
            }, unitOfWork, LogSource.PersonService);
            return goodImg;
        }
    }
}