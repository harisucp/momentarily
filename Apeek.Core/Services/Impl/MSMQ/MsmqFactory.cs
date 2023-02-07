using Apeek.Common;
using Apeek.Common.Interfaces;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public interface IMsmqFactory : IDependency
    {
        IMsmqClient<MsmqEnvelope> GetMsmqClientForImageProcessing();
    }
    public class MsmqFactory : IMsmqFactory
    {
        public IMsmqClient<MsmqEnvelope> GetMsmqClientForImageProcessing()
        {
            var msmqParams = Ioc.Get<ISettingsDataService>().GetMsmqProcessingParams();
            return new MsmqClient<MsmqEnvelope, bool>(msmqParams);
        }
    }
}