using Appointment.Domain.Model;
using AppointmentClient;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Type = System.Type;

namespace Appointment.GrpcClient.Converters
{
    public class ToAppointmentModel : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AppointmentModel);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var concreteValue = (AppointmentRequest)value;

            return new AppointmentModel
            {
                Id = concreteValue.Id,
                Name = concreteValue.Name,
                Description = concreteValue.Description,
                Location = concreteValue.Location,
                Url = concreteValue.Url,
                StartDate = concreteValue.StartDate.ToDateTime().ToLocalTime(),
                EndDate = concreteValue.EndDate.ToDateTime().ToLocalTime()
            };
        }
    }
}
