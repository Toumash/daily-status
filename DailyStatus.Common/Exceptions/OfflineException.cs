using System;

namespace DailyStatus.Common.Exceptions
{
    [Serializable]
    public class OfflineException : TogglApiException
    {
        public OfflineException() { }
        public OfflineException(string message) : base(message) { }
        public OfflineException(string message, Exception inner) : base(message, inner) { }
        protected OfflineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
