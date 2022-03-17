using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Appointment.Resources
{
    [Serializable]
    public class ExpectedException : Exception
    {
        public string ErrorCode { get; set; }
        public ExpectedException()
        {
        }

        public ExpectedException(string message) : base(message)
        {
        }
        public ExpectedException(string ErrorCode, string message) : base(message)
        {
            this.ErrorCode = ErrorCode;
        }
        public ExpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
