using System.Collections.Generic;
using System.Web;

namespace Apeek.Common.HttpContextImpl
{
    public class NoHttpSessionState : HttpSessionStateBase
    {
        public override string SessionID
        {
            get { return null; }
        }

        private readonly Dictionary<string, object> _session = new Dictionary<string, object>();

        public override object this[string name]
        {
            get
            {
                return _session[name];
            }
            set
            {
                _session[name] = value;
            }
        }
    }
}