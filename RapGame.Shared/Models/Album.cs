using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapGame.Models
{
    public class Album
    {
        public int Id {  get; set; }
        
        public required string Nome { get; set; } = string.Empty;

        public required int QuantidadeFaixas { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public required DateTime AlbumDate { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public string FaixaMaisPopular { get; set; } = "Indefinida";

        [JsonIgnore]
        public virtual ICollection<AlbumArtista> AlbumArtistas {get; set;} = new List<AlbumArtista>();
        
        public List<AlbumParticipacao> Participacoes { get; set; } = new();
    }
}

