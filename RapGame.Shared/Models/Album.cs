using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapGame.Models
{
    public class Album
    {
        public int Id {  get; set; }
        
        public required string Nome { get; set; } = string.Empty;

        public required int QuantidadeFaixas { get; set; }

        public required DateTime AlbumDate { get; set; }

        public string? FaixaMaisPopular { get; set; } = "Indefinida";

        public virtual ICollection<AlbumArtista> AlbumArtistas {get; set;} = new List<AlbumArtista>();
        
        public List<AlbumParticipacoes> Participacoes { get; set; } = new();
    }
}

