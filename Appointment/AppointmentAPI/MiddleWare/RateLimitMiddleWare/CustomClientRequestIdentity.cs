using AspNetCoreRateLimit;

namespace AppointmentAPI.MiddleWare.RateLimitMiddleWare
{
    public class CustomClientRequestIdentity : ClientRequestIdentity
    {
        public string UserId { get; set; }
    }
}