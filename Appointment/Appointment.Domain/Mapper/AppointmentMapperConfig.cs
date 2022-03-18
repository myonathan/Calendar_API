using Appointment.Domain.Converters;
using Appointment.Domain.Model;
using Nelibur.ObjectMapper;
using System.Collections.Generic;
using System.ComponentModel;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;

namespace Appointment.Domain.ConverterMapperConfigs
{
    public class AppointmentMapperConfig
    {
        public static void InitateMapper()
        {
            TypeDescriptor.AddAttributes(typeof(AppointmentModel), new TypeConverterAttribute(typeof(ToDbAppointmentConverter)));
            TinyMapper.Bind<AppointmentModel, AppointmentEntity>();
            TinyMapper.Bind<List<AppointmentModel>, List<AppointmentEntity>>();

            TypeDescriptor.AddAttributes(typeof(AppointmentEntity), new TypeConverterAttribute(typeof(FromDbAppointmentConverter)));
            TinyMapper.Bind<AppointmentEntity, AppointmentModel>();
            TinyMapper.Bind<List<AppointmentEntity>, List<AppointmentModel>>();
        }
    }
}
