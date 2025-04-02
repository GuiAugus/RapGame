using Microsoft.EntityFrameworkCore;
using RapGame.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RapGame.Data
{
    public class RapGameDbContext : DbContext
    {
        public DbSet<Album> Albuns { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<AlbumArtista> AlbumArtistas { get; set; } // Corrigido o nome da propriedade

        public RapGameDbContext(DbContextOptions<RapGameDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento principal (Artista <-> Album)
            modelBuilder.Entity<AlbumArtista>()
                .HasKey(aa => new { aa.AlbumId, aa.ArtistId });

            modelBuilder.Entity<AlbumArtista>()
                .HasOne(aa => aa.Album)
                .WithMany(a => a.AlbumArtistas)
                .HasForeignKey(aa => aa.AlbumId);

            modelBuilder.Entity<AlbumArtista>()
                .HasOne(aa => aa.Artista)
                .WithMany(a => a.AlbumArtistas)
                .HasForeignKey(aa => aa.ArtistId);

            // Relacionamento de participações (Artista convidado <-> Album)
            modelBuilder.Entity<AlbumParticipacao>()
                .HasKey(ap => new { ap.AlbumId, ap.ArtistaId });

            modelBuilder.Entity<AlbumParticipacao>()
                .HasOne(ap => ap.Album)
                .WithMany(a => a.AlbumParticipacoes)
                .HasForeignKey(ap => ap.AlbumId);

            modelBuilder.Entity<AlbumParticipacao>()
                .HasOne(ap => ap.Artista)
                .WithMany(a => a.AlbumParticipacoes)
                .HasForeignKey(ap => ap.ArtistaId);
        }
    }
}
