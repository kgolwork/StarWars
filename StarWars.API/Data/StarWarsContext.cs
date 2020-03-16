using Microsoft.EntityFrameworkCore;
using StarWars.API.Models;

namespace StarWars.API.Repository
{
    public class StarWarsContext : DbContext
    {
        public StarWarsContext(DbContextOptions<StarWarsContext> options) : base(options) { }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<CharacterEpisode> CharacterEpisodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Character>()
                .Property(c => c.Name)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Entity<Friendship>()
                .HasKey(f => new { f.CharacterId, f.FriendId });

            builder.Entity<Friendship>()
                .HasOne(f => f.Character)
                .WithMany(mu => mu.Friends)
                .HasForeignKey(f => f.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Episode>()
                .Property(c => c.Name)
                    .HasMaxLength(255)
                    .IsRequired();

            builder.Entity<CharacterEpisode>()
                .HasKey(c => new { c.CharacterId, c.EpisodeId });

            builder.Entity<CharacterEpisode>()
                .HasOne(c => c.Character)
                .WithMany(mu => mu.Episodes)
                .HasForeignKey(c => c.CharacterId);
        }
    }
}