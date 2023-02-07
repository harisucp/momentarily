﻿using Apeek.Common;
        // GET: Frontend/Admin
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;
            TempData["NoAccessFooter"] = "NoAccess";
            //if (Convert.ToBoolean(Session["IsAdmin"]) == false)
            //{
            _helper.LogOff();
            //Session["IsAdmin"] = true;
            Response.Cookies.Remove("AdminLogin");
            //    return RedirectToAction("Index");
            //}
            //if (ContextService.IsUserAuthenticated)
            //    return Redirect(QuickUrl.UserProfileUrl());
            //ViewBag.RedirectTo = ReturnUrl;
            var shape = _shapeFactory.BuildShape(null, new LoginModel(), PageName.Admin.ToString());
            if (Request.Cookies["AdminLogin"] != null)
            {
                shape.ViewModel.RememberMe = true;
            }

            //if (!_helper.ExternalLoginError) return DisplayShape(shape);
            //shape.IsError = _helper.ExternalLoginError;
            // shape.Message = "Failure to login using external system.";
            Session["IsAdmin"] = true;
            //var lists=_helper.getCampaignList("hello@momentarily.com");
            TempData["NoAccessFooter"] = "NoAccess";
            if (ContextService.IsUserAuthenticated)
                {
                    shape.ViewModel.RememberMe = false;
                    Response.Cookies["AdminLogin"].Expires = DateTime.Now.AddDays(-1);
                }

                //redirectTo = QuickUrl.UserPwdUrl();
                //if (string.IsNullOrWhiteSpace(loginResult.User.Email))
                //    redirectTo = QuickUrl.UserEmailUrl(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(QS.source, "tmp_pwd") });
                //return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
            }
    }