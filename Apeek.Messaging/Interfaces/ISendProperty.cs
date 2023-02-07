using Apeek.Common.Interfaces;
namespace Apeek.Messaging.Interfaces
{
    public interface ISendProperty : IDependency
    {
        string ProviderHost { get; set; }
        int? ProviderPort { get; set; }
        string Login { get; set; }
        string Pwd { get; set; }
    }
}