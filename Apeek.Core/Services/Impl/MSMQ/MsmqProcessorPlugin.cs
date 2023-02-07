using Apeek.Common.EventManager.DataTracker;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public abstract class MsmqProcessorPlugin
    {
        public abstract string PluginName { get; }
        protected abstract int ProcessEnvelop(MsmqEnvelope request);
        public MsmqEnvelope Answer { get; protected set; }
        public int Process(MsmqEnvelope envelope)
        {
            if (envelope.Destination == PluginName)
            {
                return ProcessEnvelop(envelope);
            }
            return ProcessStatus.Unprocessed;
        }
    }
}