using System.ComponentModel.DataAnnotations;

namespace RapGame.Models
{
    public class Artista
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public required String Nome { get; set; }

        public List<AlbumArtista> AlbumArtistas { get; set; } = new();

        public List<AlbumParticipacao> Participacoes { get; set; } = new();
    }
}
