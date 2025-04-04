using System.ComponentModel.DataAnnotations;

namespace RapGame.Models
{
    public class Artista
    {
        public int Id { get; set; }

        public required String Nome { get; set; }

        public virtual ICollection<AlbumArtista> AlbumArtistas { get; set; } = new List<AlbumArtista>();

        public List<AlbumParticipacoes> Participacoes { get; set; } = new();
    }
}
