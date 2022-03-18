using Appointment.Common.Interfaces;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;

namespace Appointment.Domain.User.UserAuth
{
    public interface IAppointmentMainRepo : IGenericRepository<AppointmentEntity>
    {

    }
}
