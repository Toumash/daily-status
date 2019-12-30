using System;

namespace DailyStatus.Common.Exceptions
{
    [Serializable]
    public class TogglApiException : Exception
    {
        public TogglApiException() { }
        public TogglApiException(string message) : base(message) { }
        public TogglApiException(string message, Exception inner) : base(message, inner) { }
        protected TogglApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
