using System;
using System.Net.Http;
using System.Web.Http;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.UrlHelpers;

namespace Apeek.Web.Infrastructure
{
    /*
 $.ajax({
                 url: 'http://tophands.com/api/BackendServices',
                 cache: false,
                 data: JSON.stringify({'ParentId':'5','ServiceId':'2','Name':'аудитор'}),
                 dataType: "json",
                 type: 'PUT',
                 contentType: 'application/json; charset=utf-8',
                 success: function (obj) {
                     console.log(obj);                      
                         
                 },
                 error: function (response) {
                     console.log(response);
                     alert('Error');
                 }
             });
 */
    public abstract class BaseApiController<TModel, TId> : ApiController
    {
        protected IUserAccessController UserAccessController { get; set; }

        public BaseApiController()
        {
            UserAccessController = Ioc.Get<IUserAccessController>();
        }

        protected int? UserId
        {
            get { return UserAccessController.UserId; }
        }

        private QuickUrl _quickUrl;

        protected QuickUrl QuickUrl
        {
            get
            {
                if (_quickUrl == null)
                {
                    _quickUrl = new QuickUrl(new HttpUrlHelper(Url), new UrlGenerator());
                }
                return _quickUrl;
            }
        }

        [System.Web.Http.ActionName("Get")]
        public virtual HttpResponseMessage Get(TId id)
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.ActionName("Get")]
        public virtual HttpResponseMessage Get()
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.ActionName("Get")]
        public virtual HttpResponseMessage Post(TModel apiModel)
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.ActionName("Get")]
        [HttpDelete]
        public virtual HttpResponseMessage Delete(TId id)
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.ActionName("Get")]
        [HttpDelete]
        public virtual HttpResponseMessage Delete(TModel id)
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.ActionName("Get")]
        public virtual HttpResponseMessage Put(TModel apiModel)
        {
            throw new NotImplementedException();
        }
    }
}