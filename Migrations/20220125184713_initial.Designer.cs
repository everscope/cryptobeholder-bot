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
    [Migration("20220125184713_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CryptoBeholderBot.TraceSettings", b =>
                {
                    b.Property<string>("CoinId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("AbsoluteMax")
                        .HasPrecision(12, 10)
                        .HasColumnType("decimal(12,10)");

                    b.Property<decimal?>("AbsoluteMin")
                        .HasPrecision(12, 10)
                        .HasColumnType("decimal(12,10)");

                    b.Property<decimal?>("Persent")
                        .HasPrecision(2, 2)
                        .HasColumnType("decimal(2,2)");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("TracingMode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CoinId");

                    b.ToTable("TracesSettings");
                });

            modelBuilder.Entity("CryptoBeholderBot.TrackedCoin", b =>
                {
                    b.Property<string>("CoinId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<string>("Coin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CoinId");

                    b.HasIndex("ChatId");

                    b.ToTable("TrackedCoins");
                });

            modelBuilder.Entity("CryptoBeholderBot.User", b =>
                {
                    b.Property<int>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChatId"), 1L, 1);

                    b.Property<string>("VsCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChatId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CryptoBeholderBot.TraceSettings", b =>
                {
                    b.HasOne("CryptoBeholderBot.TrackedCoin", "TrackedCoin")
                        .WithOne("TraceSettings")
                        .HasForeignKey("CryptoBeholderBot.TraceSettings", "CoinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrackedCoin");
                });

            modelBuilder.Entity("CryptoBeholderBot.TrackedCoin", b =>
                {
                    b.HasOne("CryptoBeholderBot.User", "User")
                        .WithMany("TrackedCoins")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoBeholderBot.TrackedCoin", b =>
                {
                    b.Navigation("TraceSettings")
                        .IsRequired();
                });

            modelBuilder.Entity("CryptoBeholderBot.User", b =>
                {
                    b.Navigation("TrackedCoins");
                });
#pragma warning restore 612, 618
        }
    }
}
