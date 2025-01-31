using Microsoft.EntityFrameworkCore;
using JustTip.Core.Models;

namespace JustTip.Infrastructure.Data
{
    public class JustTipDbContext : DbContext
    {
        public JustTipDbContext(DbContextOptions<JustTipDbContext> options)
            : base(options)
        {
        }

        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<TipAllocation> TipAllocations => Set<TipAllocation>();
        public DbSet<Shift> Shifts => Set<Shift>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Business configuration
            modelBuilder.Entity<Business>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(100);
                entity.HasOne(e => e.Business)
                    .WithMany(b => b.Employees)
                    .HasForeignKey(e => e.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Shift configuration
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Year).IsRequired();
                entity.Property(e => e.Month).IsRequired();
                entity.Property(e => e.WorkedDays).HasColumnType("nvarchar(max)");
                
                entity.HasOne(s => s.Employee)
                    .WithMany(e => e.Shifts)
                    .HasForeignKey(s => s.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TipAllocation configuration
            modelBuilder.Entity<TipAllocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DistributionDate).IsRequired();
                entity.Property(e => e.PeriodStart).IsRequired();
                entity.Property(e => e.PeriodEnd).IsRequired();
                entity.Property(e => e.DaysWorked).IsRequired();
                entity.HasOne(t => t.Employee)
                    .WithMany(e => e.TipAllocations)
                    .HasForeignKey(t => t.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 