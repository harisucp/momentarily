using System;
using System.Web;
using System.Web.Security;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Interfaces;
using Apeek.Core;
using Apeek.Core.Services;
using Apeek.Entities.Web;
using Newtonsoft.Json;

namespace Apeek.Web.Infrastructure
{
    public class UserAccessController : IUserAccessController
    {
        public int? UserId
        {
            get
            {
                int? userId = null;
                
                //from auth cooky
                if(ContextService.IsUserAuthenticated)
                    userId = ContextService.AuthenticatedUser.UserId;
                else
                {
                    //do not allow for not authenticated users use query string params
                    return null;
                }
                
                //from query string
                if (ContextService.FromQueryString.UserId.HasValue)
                {
                    userId = ContextService.FromQueryString.UserId.Value;
                }

                return userId;
            }
        }

        /// <summary>
        /// If the editing user is null we check if user has privilege to accessToUserID
        /// If the editing user is authenticated user access will true. 
        /// Else we check if user has privilege to accessToUserID
        /// </summary>
        /// <param name="privilege">Privilege name</param>
        /// <param name="accessToUserID">Authenticated user or user from query string</param>
        public bool HasAccess(string privilege, int? accessToUserID = null)
        {
            if (ContextService.IsUserAuthenticated)
            {
                if (accessToUserID.HasValue)
                {
                    if (ContextService.AuthenticatedUser.UserId == accessToUserID.Value)
                    {
                        return true;
                    }
                }

                return ContextService.AuthenticatedUser.IsInRole(privilege);
            }

            return false;
        }

        public bool AuthenticateUser(int personId)
        {
            var authTicket = CreateAuthTicket(personId);

            if (authTicket != null)
            {
                var cookie = CreateAuthCookie(authTicket);

                HttpContext.Current.Response.Cookies.Add(cookie);

                //to have authenticated user context we imidiately set it
                PostAuthRequest();

                return true;
            }
            return false;
        }

        private FormsAuthenticationTicket CreateAuthTicket(int personId)
        {
            IAccountDataService service = Ioc.Get<IAccountDataService>();
            var person = service.GetUser(personId);
            if (person != null)
            {
                var privileges = service.GetUserPrivilages(person.Id);

                var serializeModel = new ApeekPrincipalSerializeModel();
                serializeModel.UserId = person.Id;
                serializeModel.EmailAddress = person.Email;
                serializeModel.roles = privileges;
                serializeModel.Version = GetCurrentCookieVersion();

                string userData = JsonConvert.SerializeObject(serializeModel);
                return new FormsAuthenticationTicket(
                    1,
                    person.Id.ToString(),
                    DateTime.Now,
                    DateTime.Now.AddDays(30),
                    false,
                    userData);
            }

            return null;
        }

        private HttpCookie CreateAuthCookie(FormsAuthenticationTicket authenticationTicket)
        {
            string encTicket = FormsAuthentication.Encrypt(authenticationTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            //faCookie.Domain = ApeekController.CurrentCookieDns;

            return faCookie;
        }

        public void SignOutUser()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            cookie.Domain = ApeekController.CurrentCookieDns;
            cookie.Expires = DateTime.Now.AddMinutes(-5);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void UpdateUser(string newEmail)
        {
            if (ContextService.AuthenticatedUser.UserId == UserId)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                var authTicket = FormsAuthentication.Decrypt(cookie.Value);
                var serializeModel = JsonConvert.DeserializeObject<ApeekPrincipalSerializeModel>(authTicket.UserData);

                serializeModel.EmailAddress = newEmail;

                string userData = JsonConvert.SerializeObject(serializeModel);
                // Store UserData inside the Forms Ticket with all the attributes
                // in sync with the web.config
                var newAuthTicket = new FormsAuthenticationTicket(authTicket.Version,
                    authTicket.Name,
                    authTicket.IssueDate,
                    authTicket.Expiration,
                    false, // always persistent
                    userData);

                // Encrypt the ticket and store it in the cookie
                cookie.Value = FormsAuthentication.Encrypt(newAuthTicket);
                cookie.Domain = ApeekController.CurrentCookieDns;
                HttpContext.Current.Response.Cookies.Set(cookie);

                ContextService.AuthenticatedUser.EmailAddress = newEmail;
            }
        }

        public void UpdateUser(ApeekPrincipalSerializeModel serializeModel)
        {
            var newUser = new ApeekPrincipal(serializeModel);
            newUser.roles = serializeModel.roles;
            HttpContext.Current.User = newUser;
        }

        public ApeekPrincipalSerializeModel UpdateCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            var authTicket = FormsAuthentication.Decrypt(cookie.Value);

            var newAuthTicket = CreateAuthTicket(Convert.ToInt32(authTicket.Name));
            if (newAuthTicket != null)
            {
                // Encrypt the ticket and store it in the cookie
                cookie.Value = FormsAuthentication.Encrypt(newAuthTicket);
                cookie.Domain = ApeekController.CurrentCookieDns;
                HttpContext.Current.Response.AppendCookie(cookie);
                
                return JsonConvert.DeserializeObject<ApeekPrincipalSerializeModel>(newAuthTicket.UserData);
            }
            return null;
        }

        public void PostAuthRequest()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                var serializeModel = JsonConvert.DeserializeObject<ApeekPrincipalSerializeModel>(authTicket.UserData);
                if (!IsCookieVersionValid(serializeModel))
                {
                    serializeModel = UpdateCookie();
                }
                UpdateUser(serializeModel);
            }
            else
            {
                HttpContext.Current.User = new ApeekPrincipal(0, "");
            }
        }

        private bool IsCookieVersionValid(ApeekPrincipalSerializeModel model)
        {
            return model.Version == GetCurrentCookieVersion();
        }

        private int GetCurrentCookieVersion()
        {
            return typeof (UserAccessController).Assembly.GetName().Version.GetHashCode();
        }
    }
}