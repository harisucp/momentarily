﻿using System.Web.Mvc;
using System.Web;

namespace Momentarily.Web.Areas.Frontend.Controller
            if (!_helper.UserHasAccess())

            var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessage.ToString());

            return shape != null ? DisplayShape(shape) : RedirectToHome();
            var result = _helper.GetConversation(Convert.ToInt32(userId));
            if (result != null &&  result.Messages!=null&& result.Messages.Count > 0)
            {
                messagelist = result.Messages.Where(x => x.IsRead == false).OrderBy(x => x.DateCreated).ToList();
                //messagelist.Remove("");
                _helper.SetIsRead(Convert.ToInt32(userId));
                {
            }
                var result = _helper.GetConversation(Convert.ToInt32(userId));
                if (result != null && result.Messages.Count > 0)

    