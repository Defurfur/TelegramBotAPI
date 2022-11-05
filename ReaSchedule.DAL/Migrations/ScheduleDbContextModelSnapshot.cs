﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ReaSchedule.DAL;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    [DbContext(typeof(ScheduleDbContext))]
    partial class ScheduleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("ClassElementId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClassType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OrdinalNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Professor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ScheduleDayId")
                        .HasColumnType("integer");

                    b.Property<string>("Subgroup")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleDayId");

                    b.ToTable("ReaClasses");
                });

            modelBuilder.Entity("ReaSchedule.Models.ReaGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ReaGroups");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<string>("DayOfWeekName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ScheduleWeekId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleWeekId");

                    b.ToTable("ScheduleDays");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleWeek", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ReaGroupId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("WeekEnd")
                        .HasColumnType("date");

                    b.Property<DateOnly>("WeekStart")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ReaGroupId");

                    b.ToTable("ScheduleWeeks");
                });

            modelBuilder.Entity("ReaSchedule.Models.ReaClass", b =>
                {
                    b.HasOne("ReaSchedule.Models.ScheduleDay", null)
                        .WithMany("ReaClasses")
                        .HasForeignKey("ScheduleDayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleDay", b =>
                {
                    b.HasOne("ReaSchedule.Models.ScheduleWeek", "ScheduleWeek")
                        .WithMany("ScheduleDays")
                        .HasForeignKey("ScheduleWeekId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleWeek");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleWeek", b =>
                {
                    b.HasOne("ReaSchedule.Models.ReaGroup", null)
                        .WithMany("ScheduleWeeks")
                        .HasForeignKey("ReaGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReaSchedule.Models.ReaGroup", b =>
                {
                    b.Navigation("ScheduleWeeks");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleDay", b =>
                {
                    b.Navigation("ReaClasses");
                });

            modelBuilder.Entity("ReaSchedule.Models.ScheduleWeek", b =>
                {
                    b.Navigation("ScheduleDays");
                });
#pragma warning restore 612, 618
        }
    }
}
