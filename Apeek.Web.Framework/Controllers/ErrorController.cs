using System.Web.Mvc;
using Apeek.Common;
using Apeek.Core.Interfaces;
namespace Apeek.Web.Framework.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly IShapeFactory _shapeFactory;
        public ErrorController(IShapeFactory shapeFactory)
        {
            _shapeFactory = shapeFactory;
        }
        [HttpGet]
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            var shape = _shapeFactory.BuildShape(null, string.Empty, PageName.NotFound.ToString());
            return View(shape);
        }
    }
}