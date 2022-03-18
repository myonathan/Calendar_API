using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Appointment.Utilities
{
    public static class IdentityExtensions
    {
        public static int GetId(this IIdentity identity)
        {
            var id = ((ClaimsIdentity)identity).FindFirst("sub").Value;

            return ConvertHelper.ToInt(id);
        }

        public static string GetUserName(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("User_Name").Value;
        }

        public static string GetDisplayName(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("DisplayName").Value;
        }

        public static Guid GetCookieId(this IIdentity identity)
        {
            var cookieId = ((ClaimsIdentity)identity).FindFirst("CookieId").Value;

            return ConvertHelper.ToGuid(cookieId);
        }

        public static Guid GetApiCookieId(this IIdentity identity)
        {
            var apiCookieId = ((ClaimsIdentity)identity).FindFirst("ApiCookieId").Value;

            return ConvertHelper.ToGuid(apiCookieId);
        }

        public static List<int> GetRole(this IIdentity identity)
        {
            var role = ((ClaimsIdentity)identity).FindFirst("Role").Value;

            var roles = ConvertHelper.DeSerializeFromString<List<Role>>(role);

            if (roles != null && roles.Any())
                return roles.Select(x => x.Id).ToList();
            return null;
        }

        public static string GetUserTimeZone(this IIdentity identity)
        {
            var val = ((ClaimsIdentity)identity).FindFirst("myTimeZone").Value;

            var timezone = ConvertHelper.ToString(val);
            if (string.IsNullOrEmpty(timezone) || timezone.Equals("Singapore Standard Time"))
            {
                return "Asia/Singapore";
            }
            else
                return timezone;
        }
        public static string GetMySecret(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("mySecret").Value;
        }

        public static string GetUserLocalization(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("myLocale").Value;
        }
    }


    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}
