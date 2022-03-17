using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Appointment.DataAccess.Model;

#nullable disable

namespace Appointment.DataAccess.MSSQL
{
    public partial class LyteVenture_CalendarContext : DbContext
    {
        public LyteVenture_CalendarContext()
        {
        }

        public LyteVenture_CalendarContext(DbContextOptions<LyteVenture_CalendarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Model.Appointment> Appointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\localdb;Database=LyteVenture_Calendar;integrated security=False;user id=lyteventure;password=qwerty12345");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
