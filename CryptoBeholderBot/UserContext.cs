﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    internal class UserContext:DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<TrackedCoin> TrackedCoins { get; set; }
        public DbSet<TraceSettings> TracesSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().OwnsMany(
                p => p.TrackedCoins, b =>
                {
                    b.OwnsOne(b => b.TraceSettings);;
                });

            base.OnModelCreating(modelBuilder);

        }
                
    }
}
