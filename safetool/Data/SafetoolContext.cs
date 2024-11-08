﻿using safetool.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Metrics;

namespace safetool.Data
{
    public class SafetoolContext : DbContext
    {
        public SafetoolContext(DbContextOptions<SafetoolContext> options) : base(options) 
        { 
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<PPE> PPEs { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<RiskLevel> RiskLevels { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<GeneralParameter> GeneralParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Principal
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(50);
            });

            //Dependent
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.UserName)
                .HasMaxLength(50);

                entity.HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleID);
            });

            // Principal
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                 .HasMaxLength(50);

                entity.Property(e => e.Acronym)
                .HasMaxLength(50);
            });

            // Dependent
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(100);

                entity.HasOne(d => d.Location)
                .WithMany(p => p.Areas)
                .HasForeignKey(d => d.LocationID);
            });

            //Principal
            modelBuilder.Entity<DeviceType>(entity =>
            {
                entity.ToTable("DeviceType");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(50);
            });

            // Principal
            modelBuilder.Entity<PPE>(entity =>
            {
                entity.ToTable("PPE");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(100);

                entity.Property(e => e.Image)
                .HasMaxLength(500);
            });

            // Principal
            modelBuilder.Entity<Risk>(entity =>
            {
                entity.ToTable("Risk");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(100);

                entity.Property(e => e.Image)
                .HasMaxLength(500);
            });

            //Principal
            modelBuilder.Entity<RiskLevel>(entity =>
            {
                entity.ToTable("RiskLevel");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Level)
                .HasMaxLength(50);
            });

            //Dependent
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                .HasMaxLength(100);

                entity.Property(e => e.Model)
                .HasMaxLength(100);

                entity.Property(e => e.Image)
                .HasMaxLength(500);

                entity.Property(e => e.Function)
                .HasMaxLength(100);

                entity.Property(e => e.SpecificFunction)
                .HasMaxLength(100);

                entity.Property(e => e.EmergencyStopImage)
                .HasMaxLength(500);

                entity.Property(e => e.TypeSafetyDevice)
                .HasMaxLength(100);

                entity.Property(e => e.FunctionSafetyDevice)
                .HasMaxLength(100);

                entity.HasOne(d => d.Area)
                .WithMany(p => p.Devices)
                .HasForeignKey(d => d.AreaID);

                entity.HasOne(d => d.DeviceType)
                .WithMany(p => p.Devices)
                .HasForeignKey(d => d.DeviceTypeID);

                entity.HasMany(d => d.PPEs)
                .WithMany(p => p.Devices)
                .UsingEntity(j => j.ToTable("DevicePPE"));

                entity.HasMany(d => d.Risks)
                .WithMany(p => p.Devices)
                .UsingEntity(j => j.ToTable("DeviceRisk"));

                entity.HasOne(d => d.RiskLevel)
                .WithMany(p => p.Devices)
                .HasForeignKey(d => d.RiskLevelID);

            });

            modelBuilder.Entity<FormSubmission>(entity =>
            {
                entity.ToTable("FormSubmission");

                entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

                entity.Property(e => e.EmployeeUID)
                .HasMaxLength(15);

                entity.Property(e => e.EmployeeName)
                .HasMaxLength(100);

                entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(100);

                entity.HasOne(fs => fs.Device)
                .WithMany(d => d.FormSubmissions)
                .HasForeignKey(fs => fs.DeviceID);
            });

            modelBuilder.Entity<GeneralParameter>(entity =>
            {
                entity.HasKey(g => g.Id);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { ID = 1, Name = "Administrador", Active = true },
                new Role { ID = 2, Name = "Operador", Active = true }
            );

            modelBuilder.Entity<RiskLevel>().HasData(
                new RiskLevel { ID = 1, Level = "Alto", Active = true },
                new RiskLevel { ID = 2, Level = "Medio", Active = true },
                new RiskLevel { ID = 3, Level = "Bajo", Active = true }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { ID = 1, UserName = "uig65332", RoleID = 1 }
            );

            modelBuilder.Entity<GeneralParameter>().HasData(
                new GeneralParameter 
                { 
                    Id = 1,
                    EmailAccount = "mats.au_zu_fa@continental-corporation.com", 
                    EmailAccountDisplayName = "IT Management",
                    EmailAccountPassword = "4pp54c0unt!!", 
                    EmailAccountUser = "uig02796@contiwan.com",
                    EmailPort = "2525", 
                    EmailServer = "SMTPHubEU.contiwan.com", 
                    EmailSsl = true
            });
        }
    }
}
