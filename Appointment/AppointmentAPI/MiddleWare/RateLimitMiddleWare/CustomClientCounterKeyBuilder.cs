using AspNetCoreRateLimit;

namespace AppointmentAPI.MiddleWare.RateLimitMiddleWare
{
    public class CustomClientCounterKeyBuilder: ICounterKeyBuilder
    {
        private readonly ClientRateLimitOptions _options;

        public CustomClientCounterKeyBuilder(ClientRateLimitOptions options)
        {
            _options = options;
        }

        public string Build(ClientRequestIdentity requestIdentity, RateLimitRule rule)
        {
            var defaultKey = $"{_options.RateLimitCounterPrefix}_{requestIdentity.ClientId}_{rule.Period}";
            var customRequestIdentity = (CustomClientRequestIdentity)requestIdentity;
            if (!string.IsNullOrEmpty(customRequestIdentity.UserId))
            {
                var userKey = $"{defaultKey}_{customRequestIdentity.UserId}";
                return userKey;
            }
            else
            {
                return defaultKey;
            }
        }
    }
}