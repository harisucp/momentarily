namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public class VkAuthController : BaseAuthController
    {
        public override string KeyEmail { get { return "email"; } }
        public override string KeyExternalId { get { return "id"; } }
        public override string KeyName { get { return "name"; } }
        public override string KeyLocation { get { return "city"; } }
        public override string KeyImageUrl { get { return ""; } }
        protected override string ImageUrl { get { return null; } }
    }
}