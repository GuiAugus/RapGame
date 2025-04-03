using System.ComponentModel.DataAnnotations;


namespace RapGame.Shared.DTOs
{
    public class ArtistaDto
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "O nome e obrigatorio")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 a 60 caracteres.")]
        public required string Nome {get; set;}
        
        public List<int> ArtistaIds { get; set; } = new();
        public List<int> ArtistaParticipacoesIds { get; set; } = new();
        
    }
}