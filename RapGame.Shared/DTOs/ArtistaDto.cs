namespace RapGame.Shared.DTOs
{
    public class ArtistaDto
    {
        public int Id {get; set;}
        public required string Nome {get; set;}
        public List<int> ArtistaIds { get; set; } = new();
        public List<int> ArtistaParticipacoesIds { get; set; } = new();
        
    }
}