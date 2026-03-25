using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Shared.Models;

namespace OffroadVehicleRentals.Api.Data;

public class VehicleRentalContext : DbContext
{
    public VehicleRentalContext(DbContextOptions<VehicleRentalContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<RepairRecord> RepairRecords { get; set; }
    public DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItems { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VehicleNumber).IsUnique();
            entity.Property(e => e.VehicleNumber).HasMaxLength(50);
            entity.Property(e => e.Make).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.CurrentHours).HasPrecision(10, 2);
            entity.Property(e => e.CurrentMileage).HasPrecision(10, 2);
            entity.Property(e => e.NextMaintenanceHours).HasPrecision(10, 2);
            entity.Property(e => e.NextMaintenanceMileage).HasPrecision(10, 2);
        });

        // Rental configuration
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.CustomerEmail).HasMaxLength(200);
            entity.Property(e => e.StartHours).HasPrecision(10, 2);
            entity.Property(e => e.EndHours).HasPrecision(10, 2);
            entity.Property(e => e.StartMileage).HasPrecision(10, 2);
            entity.Property(e => e.EndMileage).HasPrecision(10, 2);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.Rentals)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MaintenanceRecord configuration
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaintenanceType).HasMaxLength(100);
            entity.Property(e => e.HoursAtMaintenance).HasPrecision(10, 2);
            entity.Property(e => e.MileageAtMaintenance).HasPrecision(10, 2);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.MaintenanceRecords)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RepairRecord configuration
        modelBuilder.Entity<RepairRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.RepairRecords)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ChecklistTemplate configuration
        modelBuilder.Entity<ChecklistTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        // ChecklistTemplateItem configuration
        modelBuilder.Entity<ChecklistTemplateItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemText).HasMaxLength(500);

            entity.HasOne(e => e.Template)
                .WithMany(t => t.TemplateItems)
                .HasForeignKey(e => e.ChecklistTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ChecklistItem configuration
        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemText).HasMaxLength(500);
            entity.Property(e => e.CompletedBy).HasMaxLength(200);

            entity.HasOne(e => e.Rental)
                .WithMany(r => r.ChecklistItems)
                .HasForeignKey(e => e.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Vehicle)
                .WithMany()
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
