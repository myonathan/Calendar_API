using Appointment.Common.Interfaces;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;

namespace Appointment.Domain.KoobitsUser.UserAuth
{
    public interface IAppointmentMainRepo : IGenericRepository<AppointmentEntity>
    {

    }
}
