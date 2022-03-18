using Appointment.Domain.KoobitsUser.UserAuth;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentMainRepo _appointmentMainRepo;
        private readonly IRedisCacheClient _cacheService;
        private readonly IRedisDatabase _redisDatabase;
        private readonly IConfiguration _configuration;

        public AppointmentService(IAppointmentMainRepo appointmentMainRepo, IConfiguration configuration, IRedisCacheClient cacheService)
        {
            _appointmentMainRepo = appointmentMainRepo; 
            _cacheService = cacheService;
            _configuration = configuration;
            _redisDatabase = _cacheService.GetDb(_configuration.GetValue<int>("Redis:Database"));
        }
    }
}
