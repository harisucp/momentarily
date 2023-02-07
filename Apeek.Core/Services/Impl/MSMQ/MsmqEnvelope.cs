using System;
using Apeek.Common.Interfaces;
namespace Apeek.Core.Services.Impl.MSMQ
{
    [Serializable]
    public class MsmqEnvelope
    {
        public string Destination { get; set; }
        public IMsmqMessage MsmqMessage { get; set; }
        public override string ToString()
        {
            string formattedMessage = "null";
            if (MsmqMessage != null)
                formattedMessage = MsmqMessage.ToString();
            return string.Format("Destionation: {0}; Message:[{1}]", Destination, formattedMessage);
        }
    }
}