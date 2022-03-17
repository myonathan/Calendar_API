using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace AppointmentAPI.MiddleWare.RateLimitMiddleWare
{
    public class CustomRateLimitConfiguration : RateLimitConfiguration
    {
        public override ICounterKeyBuilder EndpointCounterKeyBuilder { get; }
        public CustomRateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions, IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions)
        {
            EndpointCounterKeyBuilder = new CustomClientCounterKeyBuilder(clientOptions.Value);
        }

        public override void RegisterResolvers()
        {
            base.RegisterResolvers();
            ClientResolvers.Add(new ClientPostBodyResolveContributor(ConstantRate.BodyParam));
        }

    }
}
