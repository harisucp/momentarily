using System;
namespace Apeek.Common
{
    public class ApeekException : Exception
    {
        public ApeekException(string message) : base(message)
        {
        }
        public ApeekException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }
    }
}