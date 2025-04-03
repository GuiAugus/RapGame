using System.ComponentModel.DataAnnotations;

namespace RapGame.Models
{
    public class Artista
    {
        public int Id { get; set; }

        public required String Nome { get; set; }

        public List<AlbumArtista> AlbumArtistas { get; set; } = new();

        public List<AlbumParticipacao> Participacoes { get; set; } = new();
    }
}
