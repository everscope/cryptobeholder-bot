using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    public class UserContext:DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<TrackedCoin> TrackedCoins { get; set; }
        public DbSet<TraceSettings> TracesSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=SCAT\\SQLEXPRESS;Initial Catalog=CryptoBeholder;Integrated Security=True;Encrypt=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<TrackedCoin>(b => b.TrackedCoins)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(b => b.Id);

            modelBuilder.Entity<TrackedCoin>()
                .HasOne(b => b.TraceSettings)
                .WithOne(c => c.TrackedCoin)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey<TraceSettings>(c => c.Id);

            modelBuilder.Entity<TraceSettings>()
                .Property(p => p.AbsoluteMax).HasPrecision(12, 10);
            modelBuilder.Entity<TraceSettings>()
                .Property(p => p.AbsoluteMin).HasPrecision(12, 10);
            modelBuilder.Entity<TraceSettings>()
                .Property(p => p.Persent).HasPrecision(3, 2);

            base.OnModelCreating(modelBuilder);

        }
                
    }
}
