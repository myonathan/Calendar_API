using Appointment.Common.Interfaces;
using Appointment.DataAccess.MSSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appointment.DataAccess.Core.Repository
{
    public class AppointmentGenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LyteVenture_CalendarContext _LyteVentureCalendarContext;
        internal DbSet<T> dbSet;

        public AppointmentGenericRepository(
         LyteVenture_CalendarContext lyteventureCalendarContext
         )
        {
            this._LyteVentureCalendarContext = lyteventureCalendarContext;
            this.dbSet = lyteventureCalendarContext.Set<T>();
        }
        public void Add(T entity)
        {
            _LyteVentureCalendarContext.Set<T>().Add(entity);
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                await _LyteVentureCalendarContext.Set<T>().AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }

        }

        public void AddRange(IEnumerable<T> entities)
        {
            _LyteVentureCalendarContext.Set<T>().AddRange(entities);
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entities must not be null");
            }
            try
            {
                await _LyteVentureCalendarContext.Set<T>().AddRangeAsync(entities);
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entities)} could not be saved: {ex.Message}");
            }
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _LyteVentureCalendarContext.Set<T>().Where(expression);
        }
        public IEnumerable<T> GetAll()
        {
            return _LyteVentureCalendarContext.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _LyteVentureCalendarContext.Set<T>().Find(id);
        }
        public void Remove(T entity)
        {
            _LyteVentureCalendarContext.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _LyteVentureCalendarContext.Set<T>().RemoveRange(entities);
        }
        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                _LyteVentureCalendarContext.Update(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(entity);
            }
        }

        public void Save()
        {
            _LyteVentureCalendarContext.SaveChanges();
        }
        public IQueryable<T> GetQueryable()
        {
            return _LyteVentureCalendarContext.Set<T>().AsQueryable<T>();
        }

        public IQueryable<T> GetNoTrackingQueryable()
        {
            return _LyteVentureCalendarContext.Set<T>().AsNoTracking<T>().AsQueryable<T>();
        }
        public async Task SaveAsync()
        {
            await _LyteVentureCalendarContext.SaveChangesAsync();
        }
    }
}
