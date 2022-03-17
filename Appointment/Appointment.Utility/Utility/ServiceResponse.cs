using Newtonsoft.Json;
using System;

namespace Appointment.Utilities
{
    public class ServiceResponse
    {
        public bool IsSuccessful
        { get; set; }

        public Object Message
        { get; set; }

        public object Result
        { get; set; }
        public string Version { get; set; }

        public ServiceResponse() { }

        public ServiceResponse(bool isSuccessful) : this(isSuccessful, null) { }

        public ServiceResponse(bool isSuccessful, object result) : this(isSuccessful, isSuccessful ? "Pass" : "Fail", result) { }

        public ServiceResponse(bool isSuccessful, string message) : this(isSuccessful, message, null) { }

        public ServiceResponse(bool isSuccessful, string message, object result)
        {
            this.IsSuccessful = isSuccessful;
            this.Message = message;
            this.Result = result;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    public class ServiceResponse<T> where T : class
    {
        public bool IsSuccessful
        { get; set; }

        public Object Message
        { get; set; }

        public T Result
        { get; set; }

        public ServiceResponse() { }

        public ServiceResponse(bool isSuccessful) : this(isSuccessful, null, null) { }

        public ServiceResponse(bool isSuccessful, T result) : this(isSuccessful, isSuccessful ? "Pass" : "Fail", result) { }

        public ServiceResponse(bool isSuccessful, string message) : this(isSuccessful, message, null) { }

        public ServiceResponse(bool isSuccessful, string message, T result)
        {
            this.IsSuccessful = isSuccessful;
            this.Message = message;
            this.Result = result;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
