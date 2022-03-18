using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Appointment.Infrastructure.App_Start
{
    public static class RequestExtensions
    {
        public static int GetPageSize(this HttpRequest requestMessage,int defaultSize = 5)
        {
            return GetIntFromQueryString(requestMessage, "pageSize", defaultSize);
        }

        public static int GetPageIndex(this HttpRequest requestMessage,int defaultIndex = 0)
        {
            return GetIntFromQueryString(requestMessage, "pageIndex", defaultIndex);
        }

        public static int GetIntFromQueryString(this HttpRequest requestMessage,string key,int defaultValue)
        {
            var pair = requestMessage.Query
            .FirstOrDefault(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(pair.Value))
            {
                int value;
                if (int.TryParse(pair.Value, out value))
                {
                    return value;
                }
            }

            return defaultValue;
        }
    }
}
