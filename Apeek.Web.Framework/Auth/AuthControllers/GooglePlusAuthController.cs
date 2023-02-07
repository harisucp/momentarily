namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public class GooglePlusAuthController: BaseAuthController
    {
        public override string KeyEmail { get { return "email"; } }
        public override string KeyExternalId { get { return "id"; } }
        public override string KeyName { get { return "name"; } }
        public override string KeyLocation { get { return ""; } }
        public override string KeyImageUrl { get { return ""; } }
        protected override string Location { get { return null; } }
        protected override string ImageUrl { get { return null; } }
    }
}