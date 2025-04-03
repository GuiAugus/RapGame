namespace RapGame.Shared.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required int QuantidadeFaixas { get; set; }
        public required DateTime AlbumDate { get; set; } 
        public required string FaixaMaisPopular { get; set; }
        public List<int> ArtistaIds { get; set; } = new();
        public List<int> ArtistaParticipacoesIds { get; set; } = new();
    }
}
