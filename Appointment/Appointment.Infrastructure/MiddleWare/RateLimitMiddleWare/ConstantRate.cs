namespace Appointment.Infrastructure.MiddleWare.RateLimitMiddleWare
{
    public static class ConstantRate
    {
        public const string ClientId="appointment-client-id";
        public const string BodyParam="UserId";
        public const int StaticId = 1;
        public const string UserClaimType="user_id";
        
    }
}