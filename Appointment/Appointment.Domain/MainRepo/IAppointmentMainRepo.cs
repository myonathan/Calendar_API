using Appointment.Common.Interfaces;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;

namespace Koobits.Domain.KoobitsUser.UserAuth
{
    public interface IAppointmentMainRepo : IGenericRepository<AppointmentEntity>
    {

    }
}
