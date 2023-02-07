using System;
using System.Linq;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Common;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.Core.Services.Impl.Img
{
    public class UpdateGoodImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            string userServiceImgeToDelete = null;
            Uow.Wrap(u =>
            {
                var rep = new Repository<GoodImg>();
                var userServiceImg = rep.Table.Where(x => x.UserId == imageHandlerTarget.UserId
                                                          && x.GoodId == imageHandlerTarget.GoodId
                                                          && x.Sequence == imageHandlerTarget.Sequence
                                                          && x.Type == imageHandlerTarget.Type).Select(x => x).FirstOrDefault();
                if (userServiceImg == null)
                {
                    userServiceImg = new GoodImg()
                    {
                        UserId = imageHandlerTarget.UserId,
                        GoodId = imageHandlerTarget.GoodId.Value,
                        Sequence = imageHandlerTarget.Sequence,
                        Type = imageHandlerTarget.Type,
                        Folder = imageHandlerTarget.ImageFolder.ToString()
                    };
                }
                else
                {
                    //if old and new file names are equil we cannot delete it because it has been owerwritten by write img handler
                    if (string.Compare(userServiceImg.FileName, imageHandlerTarget.FileName, StringComparison.OrdinalIgnoreCase) != 0)
                        userServiceImgeToDelete = userServiceImg.FileName;
                }
                userServiceImg.FileName = imageHandlerTarget.FileName;
                rep.SaveOrUpdate(userServiceImg);
            });
            if (userServiceImgeToDelete != null)
                DeleteImages(userServiceImgeToDelete, imageHandlerTarget.ImageFolder);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
        private void DeleteImages(string userImagesToDelete, ImageFolder imgFolder)
        {
            var fileStorageService = Ioc.Get<IExternalFileStorageService>();
            fileStorageService.DeleteFile(imgFolder.ToString(), userImagesToDelete);
        }
    }
}