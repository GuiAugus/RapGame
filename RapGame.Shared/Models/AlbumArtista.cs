using System.Text.Json.Serialization;

namespace RapGame.Models
{
    public class AlbumArtista
    {
        public int AlbumId { get; set; }
        public required Album Album { get; set; } // Adicionando required aqui força a inicialização

        public int ArtistId { get; set; }
        public required Artista Artista { get; set; } // Adicionando required aqui também
    }
}
