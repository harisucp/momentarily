using System.Net.Http.Formatting;
using System.Web.Http;
namespace Apeek.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterFormatters(config);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }
        private static void RegisterFormatters(HttpConfiguration config)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        }
    }
}