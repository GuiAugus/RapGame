namespace RapGame.Models
{
    public class AlbumParticipacao
    {
        public int AlbumId { get; set; }
        public required Album Album { get; set; }

        public int ArtistaId { get; set; }
        public required Artista Artista { get; set; }
    }
}
