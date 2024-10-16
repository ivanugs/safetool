﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using safetool.Data;

#nullable disable

namespace safetool.Migrations
{
    [DbContext(typeof(SafetoolContext))]
    [Migration("20241015182059_UpdateGeneralParameter")]
    partial class UpdateGeneralParameter
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DevicePPE", b =>
                {
                    b.Property<int>("DevicesID")
                        .HasColumnType("int");

                    b.Property<int>("PPEsID")
                        .HasColumnType("int");

                    b.HasKey("DevicesID", "PPEsID");

                    b.HasIndex("PPEsID");

                    b.ToTable("DevicePPE", (string)null);
                });

            modelBuilder.Entity("DeviceRisk", b =>
                {
                    b.Property<int>("DevicesID")
                        .HasColumnType("int");

                    b.Property<int>("RisksID")
                        .HasColumnType("int");

                    b.HasKey("DevicesID", "RisksID");

                    b.HasIndex("RisksID");

                    b.ToTable("DeviceRisk", (string)null);
                });

            modelBuilder.Entity("safetool.Models.Area", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("LocationID")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.HasIndex("LocationID");

                    b.ToTable("Area", (string)null);
                });

            modelBuilder.Entity("safetool.Models.Device", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("AreaID")
                        .HasColumnType("int");

                    b.Property<int>("DeviceTypeID")
                        .HasColumnType("int");

                    b.Property<string>("EmergencyStopImage")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Function")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FunctionSafetyDevice")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Image")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateOnly>("LastMaintenance")
                        .HasColumnType("date");

                    b.Property<int>("LocationID")
                        .HasColumnType("int");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Operators")
                        .HasColumnType("int");

                    b.Property<int>("RiskLevelID")
                        .HasColumnType("int");

                    b.Property<string>("SpecificFunction")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TypeSafetyDevice")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.HasIndex("AreaID");

                    b.HasIndex("DeviceTypeID");

                    b.HasIndex("RiskLevelID");

                    b.ToTable("Device", (string)null);
                });

            modelBuilder.Entity("safetool.Models.DeviceType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("DeviceType", (string)null);
                });

            modelBuilder.Entity("safetool.Models.FormSubmission", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeviceID")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EmployeeUID")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.ToTable("FormSubmission", (string)null);
                });

            modelBuilder.Entity("safetool.Models.GeneralParameter", b =>
                {
                    b.Property<string>("EmailAccount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAccountDisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAccountPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAccountUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailPort")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailServer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailSsl")
                        .HasColumnType("bit");

                    b.ToTable("GeneralParameters");
                });

            modelBuilder.Entity("safetool.Models.Location", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Acronym")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Location", (string)null);
                });

            modelBuilder.Entity("safetool.Models.PPE", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Image")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.ToTable("PPE", (string)null);
                });

            modelBuilder.Entity("safetool.Models.Risk", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Image")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.ToTable("Risk", (string)null);
                });

            modelBuilder.Entity("safetool.Models.RiskLevel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("RiskLevel", (string)null);

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Active = true,
                            Level = "Alto"
                        },
                        new
                        {
                            ID = 2,
                            Active = true,
                            Level = "Medio"
                        },
                        new
                        {
                            ID = 3,
                            Active = true,
                            Level = "Bajo"
                        });
                });

            modelBuilder.Entity("safetool.Models.Role", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Role", (string)null);

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Active = true,
                            Name = "Administrador"
                        },
                        new
                        {
                            ID = 2,
                            Active = true,
                            Name = "Operador"
                        });
                });

            modelBuilder.Entity("safetool.Models.UserRole", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("RoleID");

                    b.ToTable("UserRole", (string)null);

                    b.HasData(
                        new
                        {
                            ID = 1,
                            RoleID = 1,
                            UserName = "uig65332"
                        });
                });

            modelBuilder.Entity("DevicePPE", b =>
                {
                    b.HasOne("safetool.Models.Device", null)
                        .WithMany()
                        .HasForeignKey("DevicesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("safetool.Models.PPE", null)
                        .WithMany()
                        .HasForeignKey("PPEsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DeviceRisk", b =>
                {
                    b.HasOne("safetool.Models.Device", null)
                        .WithMany()
                        .HasForeignKey("DevicesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("safetool.Models.Risk", null)
                        .WithMany()
                        .HasForeignKey("RisksID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("safetool.Models.Area", b =>
                {
                    b.HasOne("safetool.Models.Location", "Location")
                        .WithMany("Areas")
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("safetool.Models.Device", b =>
                {
                    b.HasOne("safetool.Models.Area", "Area")
                        .WithMany("Devices")
                        .HasForeignKey("AreaID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("safetool.Models.DeviceType", "DeviceType")
                        .WithMany("Devices")
                        .HasForeignKey("DeviceTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("safetool.Models.RiskLevel", "RiskLevel")
                        .WithMany("Devices")
                        .HasForeignKey("RiskLevelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");

                    b.Navigation("DeviceType");

                    b.Navigation("RiskLevel");
                });

            modelBuilder.Entity("safetool.Models.FormSubmission", b =>
                {
                    b.HasOne("safetool.Models.Device", "Device")
                        .WithMany("FormSubmissions")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("safetool.Models.UserRole", b =>
                {
                    b.HasOne("safetool.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("safetool.Models.Area", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("safetool.Models.Device", b =>
                {
                    b.Navigation("FormSubmissions");
                });

            modelBuilder.Entity("safetool.Models.DeviceType", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("safetool.Models.Location", b =>
                {
                    b.Navigation("Areas");
                });

            modelBuilder.Entity("safetool.Models.RiskLevel", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("safetool.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
