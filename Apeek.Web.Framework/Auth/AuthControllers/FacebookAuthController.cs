namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public class FacebookAuthController : BaseAuthController
    {
        public override string KeyEmail { get { return "username"; } }
        public override string KeyExternalId { get { return "id"; } }
        public override string KeyName { get { return "name"; } }
        public override string KeyLocation { get { return "location"; } }
        public override string KeyImageUrl { get { return "imageurl"; } }
    }
}