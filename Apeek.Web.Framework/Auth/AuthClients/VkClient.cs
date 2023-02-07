using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Apeek.Common.Extensions;
using DotNetOpenAuth.AspNet;
namespace Apeek.Web.Framework.Auth.AuthClients
{
    public class VkClient: IAuthenticationClient, IExternalAuthClient
    {
        public string _appId;
        public string _appSecret;
        private string _redirectUri;
        public VkClient(string appId, string appSecret)
        {
            this._appId = appId;
            this._appSecret = appSecret;
        }
        string IAuthenticationClient.ProviderName
        {
            get { return AuthClient.Vkontakte; }
        }
        void IAuthenticationClient.RequestAuthentication(HttpContextBase context, Uri redirectTo)
        {
            var APP_ID = this._appId;
            _redirectUri = context.Server.UrlEncode(redirectTo.AbsoluteUri);
            var address = String.Format(
                "https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&response_type=code",
                APP_ID, "email", _redirectUri
                );
            HttpContext.Current.Response.Redirect(address, false);
        }
        AuthenticationResult IAuthenticationClient.VerifyAuthentication(HttpContextBase context)
        {
            try
            {
                string code = context.Request["code"];
                var address = String.Format(
                    "https://oauth.vk.com/access_token?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}",
                    this._appId, this._appSecret, code, this._redirectUri);
                var response = Load(address);
                var accessToken = DeserializeJson<AccessToken>(response);
                address = String.Format(
                    "https://api.vk.com/method/users.get?uids={0}&fields=photo_50,city,bdate,sex",
                    accessToken.user_id);
                response = Load(address);
                var userData = DeserializeJson<UsersData>(response).response.First();
                address = String.Format(
                    "https://api.vk.com/method/places.getCityById?cids={0}",
                    userData.city);
                response = Load(address);
                var city = DeserializeJson<Cities>(response).response.First();
                var dict = new Dictionary<string, string>();
                dict.AddItemIfNotEmpty("id", userData.uid);
                dict.AddItemIfNotEmpty("name", userData.first_name + " " + userData.last_name);
                dict.AddItemIfNotEmpty("first_name", userData.first_name);
                dict.AddItemIfNotEmpty("last_name", userData.last_name);
                dict.AddItemIfNotEmpty("city", city.name);
                dict.AddItemIfNotEmpty("gender", userData.sex);
                dict.AddItemIfNotEmpty("birthday", userData.bdate);
                dict.AddItemIfNotEmpty("email", accessToken.email);
                dict.AddItemIfNotEmpty("access_token", accessToken.access_token);
                dict.AddItemIfNotEmpty("picture", userData.photo_50);
                return new AuthenticationResult(
                    true, (this as IAuthenticationClient).ProviderName, accessToken.user_id,
                    userData.first_name + " " + userData.last_name, dict);
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex);
            }
        }
        public static string Load(string address)
        {
            var request = WebRequest.Create(address) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static T DeserializeJson<T>(string input)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(input);
        }
        class AccessToken
        {
            public string access_token = null;
            public string user_id = null;
            public string email = null;
        }
        class UserData
        {
            public string uid = null;
            public string first_name = null;
            public string last_name = null;
            public string photo_50 = null;
            public string city = null;
            public string bdate = null;
            public string sex = null;
        }
        class UsersData
        {
            public UserData[] response = null;
        }
        class City
        {
            public string cid = null;
            public string name = null;
        }
        class Cities
        {
            public City[] response = null;
        }
        public bool UserExists(string userId, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}