using System.ComponentModel.DataAnnotations;

namespace RapGame.Models
{
    public class Album
    {
        public int id {  get; set; }

        [StringLength(60, MinimumLength = 2)]
        public required string Nome { get; set; }

        [Required]
        public int QuantidadeFaixas { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime AlbumDate { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public required string FaixaMaisPopular { get; set; }

        public List<AlbumArtista> AlbumArtistas { get; set; } = new();

        public List<AlbumParticipacao> AlbumParticipacoes { get; set; } = new();

    }
}
