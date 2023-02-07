using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    [Authorize]
    public class UserMessageController : BaseApiController<UserMessageViewModel, int>
    {
        private readonly IUserMessageService _userMessageService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly ISendMessageService _sendMessageService;
        private readonly IAccountDataService _accountDataService;
        public UserMessageController(IUserMessageService userMessageService, IUserNotificationService userNotificationService,
            ISendMessageService sendMessageService, IAccountDataService accountDataService)
        {
            _userMessageService = userMessageService;
            _userNotificationService = userNotificationService;
            _sendMessageService = sendMessageService;
            _accountDataService = accountDataService;
        }
        [HttpPost]
        public HttpResponseMessage Post(UserMessageCreateModel messageModel)
        {
            if (messageModel == null || messageModel.ReceiverId == 0 || messageModel.ReceiverId == UserId.Value
                || string.IsNullOrWhiteSpace(messageModel.Message))
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            messageModel.AuthorId = UserId.Value;
            messageModel.IsSystem = false;
            var result = _userMessageService.AddMessage(messageModel);
            if (result.CreateResult == CreateResult.Success)
            {
                var imagesPath = new List<string>();
                foreach(var image in messageModel.files)
                {
                    var imgArr = image.Split(',');

                    if(imgArr.Length > 1)
                    {
                        string part = image.Substring(0, image.IndexOf(',')).Substring(0, image.IndexOf(';'));
                        string type = part.Substring(part.LastIndexOf("/") + 1);
                        var filePath = SaveImageFromBase64(imgArr[1], type);
                        imagesPath.Add(filePath);
                    }
                }
                if(imagesPath!=null && imagesPath.Count>0)
                {
                    _userMessageService.SaveMessageImagesPath(imagesPath);
                }

                //todo not implemented
                // _userNotificationService.AddReceiveMessageNotification(messageModel.ReceiverId, messageModel.AuthorId, QuickUrl);
                    var userTo = _accountDataService.GetUser(messageModel.ReceiverId);
                var userFrom = _accountDataService.GetUser(messageModel.AuthorId);
                if (userTo != null)
                    //_sendMessageService.SendEmailGetMessageMessage(userTo.Email, userFrom.FullName ?? userFrom.Email,
                    //    QuickUrl.UserMessageConversationAbsoluteUrl(UserId.Value));
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


        private string SaveImageFromBase64(string img , string type)
        {
            string basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MessageImages");
            byte[] imageBytes = Convert.FromBase64String(img);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            string newFile = "";

            if (type.ToUpper() == "PNG")
            {
                newFile = Guid.NewGuid().ToString() + ".png";
            }
            else if (type.ToUpper() == "JPG" || type.ToUpper() == "JPEG")
            {
                newFile = Guid.NewGuid().ToString() + ".jpg";
            }
            else if (type.ToUpper() == "PLAIN")
            {
                newFile = Guid.NewGuid().ToString() + ".txt";
            }
            else
            {
                newFile = Guid.NewGuid().ToString() + "." + type;
            }
            
            var path = Path.Combine(basePath, newFile);
            bool exists = System.IO.Directory.Exists(basePath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            if (imageBytes.Length > 0)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                    stream.Flush();
                }
            }

            path = path.Replace(basePath, "").Replace("\\", "/");
            return path;
        }

        [HttpPut]
        public HttpResponseMessage Put(UserMessageUpdateModel messageModel)
        {
            if (messageModel == null || messageModel.AuthorId == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (_userMessageService.SetIsRead(messageModel.AuthorId, UserId.Value))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        
    }
}