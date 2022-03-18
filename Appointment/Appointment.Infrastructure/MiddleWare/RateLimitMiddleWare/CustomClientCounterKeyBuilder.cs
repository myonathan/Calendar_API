using AspNetCoreRateLimit;

namespace Appointment.Infrastructure.MiddleWare.RateLimitMiddleWare
{
    public class CustomClientCounterKeyBuilder : ICounterKeyBuilder
    {
        private readonly ClientRateLimitOptions _options;

        public CustomClientCounterKeyBuilder(ClientRateLimitOptions options)
        {
            _options = options;
        }

        public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            // note: we don't use identity server, so currently a constant value
            var defaultKey = $"{_options.RateLimitCounterPrefix}_{ConstantRate.ClientId}_{rule.Period}";
            return defaultKey;
        }
    }
}