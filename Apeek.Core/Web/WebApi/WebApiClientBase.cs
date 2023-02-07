using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Apeek.Common;
using Apeek.Common.Configuration;
namespace Apeek.Core.Web.WebApi
{
    public class WebApiClientBase
    {
        private string _webApiController;
        public WebApiClientBase(string webApiController)
        {
            _webApiController = webApiController;
        }
        public async void SendRequest(string action)
        {
            var url = AppSettings.GetInstance().EventManagerWebApiUrl;
            if(string.IsNullOrWhiteSpace(url))
                throw new ApeekException("Event Manager web api url is not defined");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var uriRequest = string.Format("{0}/api/{1}/{2}", url, _webApiController, action);
                HttpResponseMessage response = null;
                try
                {
                    response = await httpClient.GetAsync(uriRequest);
                }
                catch(Exception ex)
                {
                    throw new ApeekException("Event Manager WebApi is unavailable.", ex);
                }
                if (response!= null && response.IsSuccessStatusCode)
                {
                    var responce = await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    throw new ApeekException(string.Format("{0}  Status: {1} ({2})", "Event Manager WebApi is unavailable.", response.StatusCode, (int)response.StatusCode));
                }
            }
        }
        //public bool UpdateSetting(ISettingKey settingKey, string value)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var uriRequest = string.Format("{0}/api/cs/{1}/{2}", _csurl, settingKey.SettingGroup, settingKey.SettingName);
        //        var settingValue = new SettingValue(value);
        //        HttpResponseMessage response = httpClient.PutAsJsonAsync(uriRequest, settingValue).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            throw new Exception(response.Content.ReadAsStringAsync().Result);
        //        }
        //    }
        //}
    }
}