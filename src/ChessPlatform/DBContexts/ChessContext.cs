using ChessPlatform.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChessPlatform.DBContexts
{
    public class ChessContext : IdentityDbContext<User>
    {
        public ChessContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(x => x.Id);
            builder.Entity<User>().HasOne(x => x.Profile).WithOne(x => x.User).HasForeignKey<UserProfile>(x => x.Id);
            builder.Entity<User>().HasMany(x => x.Notifications).WithOne(x => x.User);
            builder.Entity<User>().HasMany(x => x.GamesAsPlayer1).WithOne(x => x.Player1);
            builder.Entity<User>().HasMany(x => x.GamesAsPlayer2).WithOne(x => x.Player2);
            builder.Entity<User>().HasMany(x => x.GamesAsWinner).WithOne(x => x.Winner);
            builder.Entity<User>().Ignore(x => x.Games);
            builder.Entity<User>().Ignore(x => x.Friends);
            
            builder.Entity<UserProfile>().HasKey(x => x.Id);
            builder.Entity<UserProfile>().HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.Id);
            builder.Entity<UserProfile>().Ignore(x => x.Rank);
            
            builder.Entity<Game>().HasKey(x => x.Id);
            builder.Entity<Game>().HasOne(x => x.Player1).WithMany(x => x.GamesAsPlayer1).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Game>().HasOne(x => x.Player2).WithMany(x => x.GamesAsPlayer2).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Game>().HasOne(x => x.Winner).WithMany(x => x.GamesAsWinner).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>().HasKey(x => x.Id);
            builder.Entity<Notification>().HasOne(x => x.User).WithMany(x => x.Notifications);

            base.OnModelCreating(builder);
        }
    }
}