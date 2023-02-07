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
    public class UpdateUserImage : ImgProcessorHandler
    {
        public override void HandleImage(ImgProcessorHandlerTarget imageHandlerTarget)
        {
            string userImageToDelete = null;
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserImg>();
                var userImage = rep.Table.Where(x => x.UserId == imageHandlerTarget.UserId
                                                     && x.Sequence == imageHandlerTarget.Sequence
                                                     && x.Type == imageHandlerTarget.Type).Select(x => x).FirstOrDefault();
                if (userImage == null)
                {
                    userImage = new UserImg()
                    {
                        UserId = imageHandlerTarget.UserId,
                        Sequence = imageHandlerTarget.Sequence,
                        Type = imageHandlerTarget.Type
                    };
                }
                else
                {
                    //if old and new file names are equil we cannot delete it because it has been owerwritten by write img handler
                    if (string.Compare(userImage.FileName, imageHandlerTarget.FileName, StringComparison.OrdinalIgnoreCase) != 0)
                        userImageToDelete = userImage.FileName;
                }
                userImage.FileName = imageHandlerTarget.FileName;
                rep.SaveOrUpdate(userImage);
            });
            if(userImageToDelete != null)
                DeleteImages(userImageToDelete);
            if (_nextImgProcessorHandler != null)
                _nextImgProcessorHandler.HandleImage(imageHandlerTarget);
        }
        private void DeleteImages(string userImagesToDelete)
        {
            var fileStorageService = Ioc.Get<IExternalFileStorageService>();
            fileStorageService.DeleteFile(ImageFolder.User.ToString(), userImagesToDelete);
        }
    }
}