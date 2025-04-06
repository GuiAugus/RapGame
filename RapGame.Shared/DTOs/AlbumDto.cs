using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Globalization;

namespace RapGame.Shared.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome e obrigatorio")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 a 60 caracteres.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "A quantidade de faixas e obrigatorio")]
        [Range(1, 100, ErrorMessage = "A quantidade de faixas deve estar entre 1 e 100")]
        public  int QuantidadeFaixas { get; set; }
        

        [JsonPropertyName("albumDate")]
        public string AlbumDateFormatted
        {
            get => AlbumDate.ToString("dd-MM-yyyy");
            set => AlbumDate = DateTime.TryParseExact(
                value,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date)
                ? date
                : default;
        }

        [Required(ErrorMessage = "A data de lançamento é obrigatória")]
        [JsonIgnore]
        public DateTime AlbumDate { get; set; }
        
        [StringLength(60, MinimumLength = 1, ErrorMessage = "A faixa deve ter entre 1 a 60 caracteres.")]
        public string? FaixaMaisPopular { get; set; }
        public List<int> ArtistaIds { get; set; } = new();
        public List<string> ArtistaPrincipais { get; set;} = new();
        public List<int> ArtistaParticipacoesIds { get; set; } = new();
        public List<string> ArtistaParticipacoes { get; set;} = new();
    }
}
