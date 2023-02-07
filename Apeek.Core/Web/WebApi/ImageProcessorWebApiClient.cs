using Apeek.Core.Web.WebApi;
namespace Apeek.Core.WebApi
{
    public class ImageProcessorWebApiClient : WebApiClientBase
    {
        public ImageProcessorWebApiClient()
            : base("ImageProcessor")
        {
        }
        public void ProcessImages()
        {
            SendRequest("Process");
        }
    }
}