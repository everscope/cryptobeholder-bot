﻿// <auto-generated />
using System;
using CryptoBeholderBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20220201144705_add-migration a")]
    partial class addmigrationa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CryptoBeholderBot.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<string>("VsCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CryptoBeholderBot.User", b =>
                {
                    b.OwnsMany("CryptoBeholderBot.TrackedCoin", "TrackedCoins", b1 =>
                        {
                            b1.Property<int>("TrackedId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("TrackedId"), 1L, 1);

                            b1.Property<string>("Coin")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int?>("UserId")
                                .HasColumnType("int");

                            b1.HasKey("TrackedId");

                            b1.HasIndex("UserId");

                            b1.ToTable("TrackedCoins");

                            b1.WithOwner()
                                .HasForeignKey("UserId");

                            b1.OwnsOne("CryptoBeholderBot.TraceSettings", "TraceSettings", b2 =>
                                {
                                    b2.Property<int>("TrackedCoinTrackedId")
                                        .HasColumnType("int");

                                    b2.Property<decimal?>("AbsoluteMax")
                                        .HasPrecision(18, 3)
                                        .HasColumnType("decimal(18,3)");

                                    b2.Property<decimal?>("AbsoluteMin")
                                        .HasPrecision(18, 3)
                                        .HasColumnType("decimal(18,3)");

                                    b2.Property<bool>("IsNotificationSent")
                                        .HasColumnType("bit");

                                    b2.Property<bool?>("MaxIsReached")
                                        .HasColumnType("bit");

                                    b2.Property<bool?>("MinIsReached")
                                        .HasColumnType("bit");

                                    b2.Property<decimal?>("Persent")
                                        .HasPrecision(5, 2)
                                        .HasColumnType("decimal(5,2)");

                                    b2.Property<bool>("PersentIsReached")
                                        .HasColumnType("bit");

                                    b2.Property<DateTime>("Time")
                                        .HasColumnType("datetime2");

                                    b2.Property<DateTime>("Timestamp")
                                        .HasColumnType("datetime2");

                                    b2.Property<int>("TracingMode")
                                        .HasColumnType("int");

                                    b2.HasKey("TrackedCoinTrackedId");

                                    b2.ToTable("TracesSettings");

                                    b2.WithOwner()
                                        .HasForeignKey("TrackedCoinTrackedId");
                                });

                            b1.Navigation("TraceSettings")
                                .IsRequired();
                        });

                    b.Navigation("TrackedCoins");
                });
#pragma warning restore 612, 618
        }
    }
}