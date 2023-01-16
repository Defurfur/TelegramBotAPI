using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using ReaSchedule.Models;
using System.Reflection;

#nullable disable
namespace ReaSchedule.DAL;

public class ScheduleDbContext : DbContext

{

    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)

        : base(options)

    {

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder
            .Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ScheduleWeek>()
            .HasMany(x => x.ScheduleDays)
            .WithOne(x => x.ScheduleWeek)
            .HasForeignKey(x => x.ScheduleWeekId);

        modelBuilder.Entity<ScheduleDay>()
            .HasOne(x => x.ScheduleWeek)
            .WithMany(x => x.ScheduleDays)
            .HasForeignKey(x => x.ScheduleWeekId);

    }

    public DbSet<ReaClass> ReaClasses{ get; set; }
    public DbSet<ReaGroup> ReaGroups{ get; set; }
    public DbSet<ScheduleWeek> ScheduleWeeks{ get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SubscriptionSettings> Settings { get; set; }
    public DbSet<Bug> Bugs{ get; set; }

}

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        d => d.ToDateTime(TimeOnly.MinValue),
        d => DateOnly.FromDateTime(d))
    { }
}