namespace Apeek.Messaging.Interfaces
{
    public class SendProperty : ISendProperty
    {
        public string ProviderHost { get; set; }
        public int? ProviderPort { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
    }
}