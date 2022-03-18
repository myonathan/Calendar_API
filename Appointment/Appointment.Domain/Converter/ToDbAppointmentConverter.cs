using Appointment.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;

namespace Appointment.Domain.Converters
{
    public class ToDbAppointmentConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AppointmentModel);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var concreteValue = (AppointmentEntity)value;

            return new AppointmentModel
            {
                Id = concreteValue.Id,
                Name = concreteValue.Name,
                Description = concreteValue.Description,
                Location = concreteValue.Location,
                Url = concreteValue.Url,
                StartDate = concreteValue.StartDate, 
                EndDate = concreteValue.EndDate
            };
        }
    }
}
