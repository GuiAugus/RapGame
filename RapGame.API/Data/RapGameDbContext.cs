using Microsoft.EntityFrameworkCore;
using RapGame.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RapGame.Data
{
    public class RapGameDbContext : DbContext
    {
        public RapGameDbContext(DbContextOptions<RapGameDbContext> options)
            : base(options)
        {
        }

        public DbSet<Album> Albuns { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<AlbumArtista> AlbumArtistas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlbumArtista>()
                .HasKey(aa => new { aa.AlbumId, aa.ArtistId});

            modelBuilder.Entity<AlbumArtista>()
                .HasOne(aa => aa.Album)
                .WithMany(a => a.AlbumArtistas)
                .HasForeignKey(aa => aa.AlbumId);

            modelBuilder.Entity<AlbumArtista>()
                .HasOne(aa => aa.Artista)
                .WithMany(a => a.AlbumArtistas)
                .HasForeignKey(aa => aa.ArtistId);
        }
    }
}
