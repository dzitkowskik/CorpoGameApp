using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CorpoGameApp.Models;
using System;

namespace CorpoGameApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerGames> PlayerGames { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<Game>().ToTable("Game");
            modelBuilder.Entity<Player>(t => 
            {
                t.ToTable("Player");
                t.Property(x => x.Id).ValueGeneratedOnAdd();
                t.Property(x => x.Score).HasDefaultValue(0);
            });

            modelBuilder.Entity<PlayerGames>().HasKey(x => new { x.PlayerId, x.GameId });
            
            modelBuilder.Entity<Game>()
                .Property<DateTime?>(m => m.EndTime)
                .IsRequired(false);

            modelBuilder.Entity<PlayerGames>()
                .HasOne(pc => pc.Player)
                .WithMany(c => c.Games)
                .HasForeignKey(pc => pc.PlayerId);

            modelBuilder.Entity<PlayerGames>()
                .HasOne(pc => pc.Game)
                .WithMany(p => p.Players)
                .HasForeignKey(pc => pc.GameId);
        
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Player>(x => x.UserForeignKey);
        }
    }
}
