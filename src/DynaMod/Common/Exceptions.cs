using System.Runtime.Serialization;
using System;

namespace DynaMod.Common
{
    [Serializable]
    public class MisconfigurationException : Exception
    {
        public MisconfigurationException() { }
        public MisconfigurationException(string message) : base(message) { }
        public MisconfigurationException(string message, Exception inner) : base(message, inner) { }
        protected MisconfigurationException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class PartitionKeyConflict : Exception
    {
        public PartitionKeyConflict() { }
        public PartitionKeyConflict(string message) : base(message) { }
        public PartitionKeyConflict(string message, Exception inner) : base(message, inner) { }
        protected PartitionKeyConflict(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
