using AspNetCoreRateLimit;

namespace Appointment.Infrastructure.MiddleWare.RateLimitMiddleWare
{
    public class CustomClientRequestIdentity : ClientRequestIdentity
    {
        public string UserId { get; set; }
    }
}