using Microsoft.EntityFrameworkCore;
using ReaSchedule.Models;

#nullable disable
namespace ReaSchedule.DAL;

public class ScheduleDbContext : DbContext

{

    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)

        : base(options)

    {

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

    



}