﻿// <auto-generated />
using System;
using BillTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BillTracker.Migrations
{
    [DbContext(typeof(BillTrackerContext))]
    [Migration("20210330154236_ExpenseTypeIdIsOptional")]
    partial class ExpenseTypeIdIsOptional
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("BillTracker.Entities.DashboardCalendarDayView", b =>
                {
                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b
                        .HasAnnotation("Relational:SqlQuery", "\r\nSELECT\r\n    ea.\"UserId\" AS \"UserId\",\r\n    SUM(e.\"Amount\") AS \"TotalAmount\",\r\n    ea.\"AddedDate\"::DATE AS \"AddedDate\"\r\nFROM \"Expenses\" AS e\r\nJOIN \"ExpensesAggregates\" AS ea ON e.\"AggregateId\" = ea.\"Id\"\r\nWHERE ea.\"IsDraft\" = false\r\nGROUP BY \r\n    ea.\"UserId\",\r\n    ea.\"AddedDate\"::DATE");
                });

            modelBuilder.Entity("BillTracker.Entities.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<Guid?>("ExpenseTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AggregateId");

                    b.HasIndex("ExpenseTypeId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpenseBillFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileUri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AggregateId");

                    b.ToTable("ExpenseBills");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpenseType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExpenseTypes");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpensesAggregate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDraft")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExpensesAggregates");
                });

            modelBuilder.Entity("BillTracker.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("ValidTo")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasAlternateKey("Token");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("BillTracker.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("WasSetup")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BillTracker.Entities.Expense", b =>
                {
                    b.HasOne("BillTracker.Entities.ExpensesAggregate", "Aggregate")
                        .WithMany("Expenses")
                        .HasForeignKey("AggregateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BillTracker.Entities.ExpenseType", "ExpenseType")
                        .WithMany("Expenses")
                        .HasForeignKey("ExpenseTypeId");

                    b.Navigation("Aggregate");

                    b.Navigation("ExpenseType");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpenseBillFile", b =>
                {
                    b.HasOne("BillTracker.Entities.ExpensesAggregate", "Aggregate")
                        .WithMany("ExpenseBillFiles")
                        .HasForeignKey("AggregateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aggregate");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpenseType", b =>
                {
                    b.HasOne("BillTracker.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpensesAggregate", b =>
                {
                    b.HasOne("BillTracker.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BillTracker.Entities.RefreshToken", b =>
                {
                    b.HasOne("BillTracker.Entities.User", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("BillTracker.Entities.RefreshToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpenseType", b =>
                {
                    b.Navigation("Expenses");
                });

            modelBuilder.Entity("BillTracker.Entities.ExpensesAggregate", b =>
                {
                    b.Navigation("ExpenseBillFiles");

                    b.Navigation("Expenses");
                });

            modelBuilder.Entity("BillTracker.Entities.User", b =>
                {
                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}
