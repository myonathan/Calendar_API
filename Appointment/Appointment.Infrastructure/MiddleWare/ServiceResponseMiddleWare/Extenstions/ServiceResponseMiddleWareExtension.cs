using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appointment.Infrastructure.MiddleWare.ServiceResponseMiddleWare.wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Appointment.Utilities;

namespace Appointment.Infrastructure.MiddleWare.ServiceResponseMiddleWare.Extension
{
    public static class ServiceResponseMiddleWareExtension
    {
        public static IApplicationBuilder UseServiceResponseWrapper(this IApplicationBuilder builder, ApiServiceResponseWrapperOptions options = default)
        {
            options ??= new ApiServiceResponseWrapperOptions();
            return builder.UseMiddleware<ServiceResponseMiddleware>(options);
        }
        public static string AllErrorsSerialized(this ModelStateDictionary modelState)
        {
            var g = modelState.Keys.SelectMany(key => modelState[key].Errors.ToList()).ToList();
            return ConvertHelper.SerializetoString<List<ModelError>>(g);

        }
    }

    public static class StringEnumExtension
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
    }

    public static class StringExtension
    {
        public static bool IsValidJson(this string text)
        {
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
                (text.StartsWith("[") && text.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(text);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }




}
