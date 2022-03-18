using Appointment.Domain.User.UserAuth;
using Appointment.Domain.Model;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nelibur.ObjectMapper;
using AppointmentEntity = Appointment.DataAccess.Entity.Appointment;
using Appointment.Resources;
using Appointment.Common.Constants;
using System.Threading.Tasks;
using Appointment.Infrastructure;

namespace Appointment.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentMainRepo _appointmentMainRepo;
        private readonly IRedisCacheClient _cacheService;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentMainRepo appointmentMainRepo, IConfiguration configuration, IRedisCacheClient cacheService, ILogger<AppointmentService> logger)
        {
            _appointmentMainRepo = appointmentMainRepo;
            _cacheService = cacheService;
            _configuration = configuration;
            _redisDatabase = _cacheService.GetDb(_configuration.GetValue<int>("Redis:Database"));
            _logger = logger;
        }

        public async Task<List<AppointmentModel>> GetAppointments(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ExpectedException(nameof(Constants.Appointment.APP04), Constants.Appointment.APP04);

            // any start dates that match with the start & end, cached 5 minutes 
            List<AppointmentEntity> entities = await _cacheService.Db0.GetValueFromCache<List<AppointmentEntity>>(string.Format(CacheKeys.Appointment.StaticAppointmentStartEndDate, start.Date.ToString("dd/MM/yyyy"), end.Date.ToString("dd/MM/yyyy"))
             , () =>{ return _appointmentMainRepo.GetNoTrackingQueryable().Where(x => x.StartDate.Date >= start.Date && x.StartDate.Date <= end.Date).ToList(); }
             , CacheKeys.Appointment.AppointmentTimespan);

            if (!entities.Any())
                throw new ExpectedException(nameof(Constants.CommonMessages.ER001), Constants.CommonMessages.ER001);

            return TinyMapper.Map<List<AppointmentEntity>, List<AppointmentModel>>(entities);
        }

        public bool DeleteAppointment(AppointmentModel appointment)
        {
            if (appointment.Id < 1)
                throw new ExpectedException(nameof(Constants.Appointment.APP01), string.Format(Constants.Appointment.APP01, appointment.Id));

            var entity = _appointmentMainRepo.GetQueryable().Where(x => x.Id == appointment.Id).SingleOrDefault();
            if (entity == null)
                throw new ExpectedException(nameof(Constants.CommonMessages.ER001), Constants.CommonMessages.ER001);

            _appointmentMainRepo.Remove(entity);
            _appointmentMainRepo.Save();

            return true;
        }

        public AppointmentModel AddAppointment(AppointmentModel appointment)
        {
            appointment.Validate();

            // if start date & end date fall between existing data within range then throw exceptin 
            var entities = _appointmentMainRepo.GetNoTrackingQueryable().Where(x =>
                    (appointment.StartDate >= x.StartDate
                     && appointment.StartDate <= x.EndDate) 
                                    ||
                     (appointment.EndDate >= x.StartDate
                     && appointment.EndDate <= x.EndDate)
            ).ToList();

            if (entities.Any())
                throw new ExpectedException(nameof(Constants.Appointment.APP02), 
                    string.Format(Constants.Appointment.APP02, 
                    "Your Appointment" + appointment.StartDate.ToUniversalTime() + " " + appointment.EndDate.ToUniversalTime() + "\n"  + string.Join(System.Environment.NewLine, 
                    entities.Select(x => (x.Name + " " + x.StartDate.ToUniversalTime().ToString("s") + " " + x.EndDate.ToUniversalTime().ToString("s"))))));

            var entity = TinyMapper.Map<AppointmentModel, AppointmentEntity>(appointment);
            entity.CreateBy = -1;
            entity.CreateDate = DateTime.Now;

            _appointmentMainRepo.Add(entity);
            _appointmentMainRepo.Save();

            appointment.Id = entity.Id;

            return appointment;
        }

        public AppointmentModel UpdateAppointment(AppointmentModel appointment)
        {
            if (appointment.Id < 1)
                throw new ExpectedException(nameof(Constants.Appointment.APP01), string.Format(Constants.Appointment.APP01, appointment.Id));

            var entity = _appointmentMainRepo.GetQueryable().Where(x => x.Id == appointment.Id).SingleOrDefault();
            if (entity == null)
                throw new ExpectedException(nameof(Constants.CommonMessages.ER001), Constants.CommonMessages.ER001);

            entity.Name = appointment.Name;
            entity.Description = appointment.Description;
            entity.Url = appointment.Description;
            entity.Location = appointment.Location;
            entity.StartDate = appointment.StartDate;
            entity.EndDate = appointment.EndDate;
            entity.UpdateBy = -1; // system 
            entity.UpdateDate = DateTime.Now;

            _appointmentMainRepo.Update(entity);
            _appointmentMainRepo.Save();

            return appointment;
        }
    }
}
