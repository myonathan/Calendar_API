using Appointment.DataAccess.Core.Repository;
using Appointment.DataAccess.MSSQL;
using Appointment.Domain.KoobitsUser.UserAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Domain.MainRepo
{
    public class AppointmentMainRepo : AppointmentGenericRepository<DataAccess.Entity.Appointment> , IAppointmentMainRepo
    {
        public AppointmentMainRepo(LyteVenture_CalendarContext context)
              : base(context)
        {

        }
    }
}
