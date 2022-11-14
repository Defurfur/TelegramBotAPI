﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ReaSchedule.DAL;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    [DbContext(typeof(ScheduleDbContext))]
    [Migration("20220910113123_InitialMigraion")]
    partial class InitialMigraion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ReaSchedule.Models.ReaClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Audition")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("ClassTimePeriod")
                        .HasColumnType("interval");

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Professor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ScheduleDayId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleDayId");

                    b.ToTable("ReaClasses");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsFree")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("ScheduleDays");
                });

            modelBuilder.Entity("ReaSchedule.Models.ReaClass", b =>
                {
                    b.HasOne("ReaSchedule.Models.ScheduleDay", null)
                        .WithMany("ReaClasses")
                        .HasForeignKey("ScheduleDayId");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleDay", b =>
                {
                    b.Navigation("ReaClasses");
                });
#pragma warning restore 612, 618
        }
    }
}