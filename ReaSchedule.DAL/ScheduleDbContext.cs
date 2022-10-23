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

    //protected override void OnModelCreating(ModelBuilder modelBuilder)

    //{

    //    base.OnModelCreating(modelBuilder);



    //    modelBuilder.Entity<MaterialSupplier>()

    //        .HasOne(a => a.PriceSource)

    //        .WithOne(b => b.MaterialSupplier)
    //    .HasForeignKey<PriceSource>(b => b.MaterialSupplierId);

    //}



    public DbSet<ReaClass> ReaClasses{ get; set; }
    public DbSet<ScheduleDay> ScheduleDays{ get; set; }

    //public DbSet<Material> Materials { get; set; }

    //public DbSet<Unit> Units { get; set; }

    //public DbSet<Supplier> Suppliers { get; set; }

    //public DbSet<Order> Orders { get; set; }

    //public DbSet<ProductMaterial> ProductMaterials { get; set; }

    //public DbSet<MaterialSupplier> MaterialSuppliers { get; set; }

    //public DbSet<OrderProduct> OrderProducts { get; set; }
    //public DbSet<PriceSource> PriceSource { get; set; }



}